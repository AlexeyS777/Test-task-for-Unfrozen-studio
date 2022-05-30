

/*
 * ����� ���������� �� ��������� ���������� � ����.
 * ��������� �������� � ������������ �������������� � ��������.
*/



using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;




public class MinerAi : MonoBehaviour
{
    /// <summary>
    /// ������ ������ ��������� ������.
    /// </summary>
    [SerializeField] private GameObject myChoiseEffect = null;
    /// <summary>
    /// ������ ������ �����.
    /// </summary>
    [SerializeField] private GameObject enemyChoiseEffect = null;
    /// <summary>
    /// ������ �����.
    /// </summary>
    [SerializeField] private Animator blood = null;


    [HideInInspector] public Vector3 startPos; // ------------------------------------------ ��������� ������� ������ (��� ������������)
    [HideInInspector] public MeshRenderer myMesh;

    private Transform myPos;
    private Animator anim;
    private GameManager gmMngr;
    private AudioSource myVoice;

    private int posIndex; // --------------------------------------------------------------- ������ ������� ���������
    public bool ready = true; //------------------------------------------------------------ ���������� ��������� � �������� true - ����������� false - �����

    public int PosIndex { get { return posIndex; } set { posIndex = value; } } // ---------- �������� ��� �������������� ������� �������
    
    
    // Start is called before the first frame update
    void Awake()
    {
        myVoice = GetComponent<AudioSource>();
        myMesh = GetComponent<MeshRenderer>();
        gmMngr = GameObject.Find("GameManager").GetComponent<GameManager>();
        myPos = GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }
    

    // ��������� ��������� ������ �����, ��� ��������� ������� �� ��������� ���������
    private void OnMouseEnter()
    {
        if (posIndex > 3 && GameManager.playerTurn && GameManager.turnStart) enemyChoiseEffect.SetActive(true);
    }



    // ���������� ��������� ������ �����, ��� ������ ������� �� ���� ���������� ���������
    private void OnMouseExit()
    {
        if (posIndex > 3 && GameManager.playerTurn && GameManager.turnStart) enemyChoiseEffect.SetActive(false);
    }


    // ����� ����� � ������� ����� ����
    private void OnMouseDown()
    {
        if (posIndex > 3 && GameManager.playerTurn && GameManager.turnStart)
        {
            gmMngr.DefendingChar = this; // ------------------------------------------- �������� ������ �������� ��������� � ������ ������������� ���������.
            StartCoroutine(gmMngr.StartBattle()); //                    ������� ��� ������ �����. 
        }
        
    }

    // -------------------------------------------------------------------------- ��������� ������� �����.
    public void BloodEffectInitialize()
    {
        blood.GetComponent<MeshRenderer>().sortingOrder = this.GetComponent<MeshRenderer>().sortingOrder + 1;
        blood.GetComponent<SkeletonMecanim>().initialFlipX = this.GetComponent<SkeletonMecanim>().initialFlipX;
        blood.GetComponent<SkeletonMecanim>().Initialize(true);
    }

    public void RandomAttack() // ----------------------------------------------- ��������� ����� ���� ����� (������-�� DoubleShift �������� ����� ����).
    {
        ready = false;

        int variantOfAttack;
        string attackName = "Miner_1";

        variantOfAttack = Random.Range(0, 3);

        switch (variantOfAttack)
        {
            case 0:
                attackName = "Miner_1";
                break;
            case 1:
                attackName = "DoubleShift";
                break;
            case 2:
                attackName = "PickaxeCharge";
                break;
            default:
                attackName = "DoubleShift";
                break;

        }

        anim.Play(attackName);
    }

    public void Damage() // --------------------------------------------------- ��������� ����� ���������� (����� ���), ������ �������� �����
    {
        ready = false;
        anim.enabled = false; //   -     -     -    -    -    -   -   -   -   - ����� ���������, ��� ����������� ���� � ������������ ��������.
        anim.enabled = true;
        this.anim.Play("Damage");
        this.blood.gameObject.SetActive(true);
        this.blood.GetComponent<MeshRenderer>().sortingOrder = myMesh.sortingOrder + 1; // ������ ����� Sorting Layer Order ������ ��� � ���������
                                                                                        // ��� ����������� �����������
        this.blood.Play("Damage");
    }

    public GameObject ChoiseEffect() // ---------------------------------------- ��������� / ���������� ������� ������ ����. ������ �� ������� ������
    {                                //                                          (����� ��� ������������ ����������, �� ���� ��������� event )
        return myChoiseEffect;
    }
    
    public void Hit() // ------------------------------------------------------- ����� - ����� ������ �������� �����, ��������� �������� �����
    {
        myVoice.Play();
        gmMngr.DefendingChar.Damage(); // -------------------------------------------- ������� ��������� - ��������� ��� ������� �������� �����
    }       

    public void EndAttack() // ------------------------------------------------- ����� - ����� ������ �������� �����
    {
        gmMngr.DefendingChar.HideBlood(); 
        ready = true;
        anim.enabled = false; //   -     -     -    -    -    -   -   -   -   - ����������� ����� ���������, ��� ����������� ���� � ������������ ��������.
        anim.enabled = true;
    }

    public void HideBlood()// ------------------------------------------------- ������� ������� �����
    {
        blood.gameObject.SetActive(false);
    }


    public IEnumerator Move(float pos) // ------------------------------------- ����������� ���������� �� ������������ ���������
    {        
        this.ready = false;
        if (myChoiseEffect.activeSelf) myChoiseEffect.SetActive(false); //      ���������� �������� ������
        if (enemyChoiseEffect.activeSelf) enemyChoiseEffect.SetActive(false); 

        if (PosIndex != 0 && PosIndex != 4) //-   -   -   -   -   -   -   -   - ��������� ��������� ������ � ���������� �� ������ ��������� ��-�� ������ ����������
        {
            Vector3 finpos = myPos.position;
            finpos.x = pos;

            anim.Play("Pull"); // -  -   -   -   -   -   -   -   -   -   -   -  ������ �������� ��������

            while (myPos.position.x != pos)
            {
                myPos.position = Vector3.MoveTowards(myPos.position, finpos, 4 * Time.deltaTime);
                yield return null;
            }            
        }
        
        this.ready = true;
    }
}
