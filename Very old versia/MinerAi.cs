

/*
 * Класс отвечающий за поведение персонажей в игре.
 * Запускает анимации и регистрирует взаимодействие с курсором.
*/



using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;




public class MinerAi : MonoBehaviour
{
    /// <summary>
    /// Эффект выбора персонажа игрока.
    /// </summary>
    [SerializeField] private GameObject myChoiseEffect = null;
    /// <summary>
    /// Еффект выбора врага.
    /// </summary>
    [SerializeField] private GameObject enemyChoiseEffect = null;
    /// <summary>
    /// Эффект крови.
    /// </summary>
    [SerializeField] private Animator blood = null;


    [HideInInspector] public Vector3 startPos; // ------------------------------------------ Стартовая позиция игрока (для передвижения)
    [HideInInspector] public MeshRenderer myMesh;

    private Transform myPos;
    private Animator anim;
    private GameManager gmMngr;
    private AudioSource myVoice;

    private int posIndex; // --------------------------------------------------------------- Индекс позиции персонажа
    public bool ready = true; //------------------------------------------------------------ Готовность персонажа к действию true - бездействие false - занят

    public int PosIndex { get { return posIndex; } set { posIndex = value; } } // ---------- Свойство для редактирования индокса позиции
    
    
    // Start is called before the first frame update
    void Awake()
    {
        myVoice = GetComponent<AudioSource>();
        myMesh = GetComponent<MeshRenderer>();
        gmMngr = GameObject.Find("GameManager").GetComponent<GameManager>();
        myPos = GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }
    

    // Включении индикации выбора врага, при наведении курсора на коллайдер персонажа
    private void OnMouseEnter()
    {
        if (posIndex > 3 && GameManager.playerTurn && GameManager.turnStart) enemyChoiseEffect.SetActive(true);
    }



    // Отключение индикации выбора врага, при выходе курсора из зоны коллайдера персонажа
    private void OnMouseExit()
    {
        if (posIndex > 3 && GameManager.playerTurn && GameManager.turnStart) enemyChoiseEffect.SetActive(false);
    }


    // Выбор врага с помощью клика мыши
    private void OnMouseDown()
    {
        if (posIndex > 3 && GameManager.playerTurn && GameManager.turnStart)
        {
            gmMngr.DefendingChar = this; // ------------------------------------------- Передаем данные игровому менеджеру о выборе защищающегося персонажа.
            StartCoroutine(gmMngr.StartBattle()); //                    Команда для начала атаки. 
        }
        
    }

    // -------------------------------------------------------------------------- Настройка эффекта крови.
    public void BloodEffectInitialize()
    {
        blood.GetComponent<MeshRenderer>().sortingOrder = this.GetComponent<MeshRenderer>().sortingOrder + 1;
        blood.GetComponent<SkeletonMecanim>().initialFlipX = this.GetComponent<SkeletonMecanim>().initialFlipX;
        blood.GetComponent<SkeletonMecanim>().Initialize(true);
    }

    public void RandomAttack() // ----------------------------------------------- Случайный выбор вида атаки (почему-то DoubleShift выпадает очень мало).
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

    public void Damage() // --------------------------------------------------- Получение урона персонажем (урона нет), запуск анимации урона
    {
        ready = false;
        anim.enabled = false; //   -     -     -    -    -    -   -   -   -   - Сброс аниматора, для исправления бага с перезапуском анимации.
        anim.enabled = true;
        this.anim.Play("Damage");
        this.blood.gameObject.SetActive(true);
        this.blood.GetComponent<MeshRenderer>().sortingOrder = myMesh.sortingOrder + 1; // задаем крови Sorting Layer Order больше чем у персонажа
                                                                                        // для корректного отображения
        this.blood.Play("Damage");
    }

    public GameObject ChoiseEffect() // ---------------------------------------- Включение / отключение эффекта выбора перс. игрока из другого класса
    {                                //                                          (Забыл как пользоваться делегатами, не могу подписать event )
        return myChoiseEffect;
    }
    
    public void Hit() // ------------------------------------------------------- Метод - метка внутри анимации атаки, запускает анимацию урона
    {
        myVoice.Play();
        gmMngr.DefendingChar.Damage(); // -------------------------------------------- Команда персонажу - защитнику для запуска анимации урона
    }       

    public void EndAttack() // ------------------------------------------------- Метод - метка внутри анимации атаки
    {
        gmMngr.DefendingChar.HideBlood(); 
        ready = true;
        anim.enabled = false; //   -     -     -    -    -    -   -   -   -   - Контрольный сброс аниматора, для исправления бага с перезапуском анимации.
        anim.enabled = true;
    }

    public void HideBlood()// ------------------------------------------------- Скрытие эффекта крови
    {
        blood.gameObject.SetActive(false);
    }


    public IEnumerator Move(float pos) // ------------------------------------- Сопрограмма отвечающая за передвижение персонажа
    {        
        this.ready = false;
        if (myChoiseEffect.activeSelf) myChoiseEffect.SetActive(false); //      Отключение эффектов выбора
        if (enemyChoiseEffect.activeSelf) enemyChoiseEffect.SetActive(false); 

        if (PosIndex != 0 && PosIndex != 4) //-   -   -   -   -   -   -   -   - Ближайшие персонажи игрока и противника не должны двигаться из-за малого расстояния
        {
            Vector3 finpos = myPos.position;
            finpos.x = pos;

            anim.Play("Pull"); // -  -   -   -   -   -   -   -   -   -   -   -  Запуск анимации движения

            while (myPos.position.x != pos)
            {
                myPos.position = Vector3.MoveTowards(myPos.position, finpos, 4 * Time.deltaTime);
                yield return null;
            }            
        }
        
        this.ready = true;
    }
}
