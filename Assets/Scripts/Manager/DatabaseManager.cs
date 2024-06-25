using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;
using System.Collections.Generic;


public enum UnitType
{
    Ground,
    Air,
    Building,
    Both
}
public enum CardType
{
    Common,
    Rare,
    Epic,
    Legendary
}
public struct UnitData
{
    public int id;
    public string name;
    public int cost;
    public UnitType Attack;
    public float health;
    public float AttackDamage;
    public float AttackSpeed;
    public float AttackRange;
    public int MoveSpeed;
    public UnitType AttackType;
    public int Population;
    public CardType cardType;

    public UnitData(int id, string name, int cost, UnitType attack, float health, float attackDamage, float attackSpeed, float attackRange, int moveSpeed, UnitType attackType, int population, CardType cardType)
    {
        this.id = id;
        this.name = name;
        this.cost = cost;
        this.Attack = attack;
        this.health = health;
        this.AttackDamage = attackDamage;
        this.AttackSpeed = attackSpeed;
        this.AttackRange = attackRange;
        this.MoveSpeed = moveSpeed;
        this.AttackType = attackType;
        this.Population = population;
        this.cardType = cardType;
    }
}


public class DatabaseManager : MonoBehaviour
{
    private static DatabaseManager _instance;

    public static DatabaseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("DatabaseManager").AddComponent<DatabaseManager>();
            }
            return _instance;
        }

    }

    [Header("ConnectionInfo")]
    [SerializeField] string _ip = "127.0.0.1";
    [SerializeField] string _dbName = "test";
    [SerializeField] string _uid = "root";
    [SerializeField] string _pwd = "1234";

    private bool _isConnectTestCompolete;

    private static MySqlConnection _dbConnection;

    Dictionary<int, UnitData> unitDataDic = new Dictionary<int, UnitData>();
    Dictionary<int, Sprite> unitSpriteDic = new Dictionary<int, Sprite>();

    public UnitData GetUnitData(int id)
    {
        return unitDataDic[id];
    }
    public Sprite GetSpriteData(int id)
    {
        return unitSpriteDic[id];
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        ConnectToDB();
    }

    public void ConnectToDB()
    {
        _isConnectTestCompolete = ConnetTest();
        SendQuery();
    }

    private void SendQuery(string queryStr, string tableName)
    {
        //SELECT관련 함수 호출
        if (queryStr.Contains("SELECT"))
        {
            DataSet dataSet = OnSelectRequest(queryStr, tableName);
            string result = DeformatResult(dataSet);
        }
    }
    private string DeformatResult(DataSet dataSet)
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (DataTable table in dataSet.Tables)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    stringBuilder.Append($"{row[column]},");
                }

                string tableRow = stringBuilder.ToString().TrimEnd(',');
                string[] columns = tableRow.Split(',');

                if (columns.Length < 11)
                {
                    stringBuilder.Clear();
                    continue;
                }

                // Dictionary에 넣기전 데이터 파싱
                try
                {
                    int id = int.Parse(columns[0]);
                    string name = columns[1];
                    int cost = int.Parse(columns[2]);
                    UnitType unitType = (UnitType)Enum.Parse(typeof(UnitType), columns[3]);
                    float health = float.Parse(columns[4]);
                    float attackDamage = float.Parse(columns[5]);
                    float attackSpeed = float.Parse(columns[6]);
                    float attackRange = float.Parse(columns[7]);
                    int moveSpeed = int.Parse(columns[8]);
                    UnitType attackType = (UnitType)Enum.Parse(typeof(UnitType), columns[9]);
                    int population = int.Parse(columns[10]);

                    CardType cardType = CardType.Common;
                    if (columns.Length == 12 && !string.IsNullOrEmpty(columns[11]))
                    {
                        cardType = (CardType)Enum.Parse(typeof(CardType), columns[11]);
                    }

                    unitDataDic.Add(id,new UnitData(
                        id,
                        name,
                        cost,
                        unitType,
                        health,
                        attackDamage,
                        attackSpeed,
                        attackRange,
                        moveSpeed,
                        attackType,
                        population,
                        cardType
                    ));
                    if(id == 5 || id == 4)
                    {
                        Sprite unitSprite = Resources.Load<Sprite>($"Sprites/UI/UnitSprite/Unit_{id}") as Sprite;
                        unitSpriteDic.Add(id, unitSprite);
                    }

                }
                catch
                {

                }

                stringBuilder.Clear();
            }
        }
        return stringBuilder.ToString();
    }

    public static DataSet OnSelectRequest(string queryStr, string tableName)
    {
        try
        {
            _dbConnection.Open();
            MySqlCommand sqlcmd = new MySqlCommand();
            sqlcmd.Connection = _dbConnection;
            sqlcmd.CommandText = queryStr;

            MySqlDataAdapter sd = new MySqlDataAdapter(sqlcmd);
            DataSet dataSet = new DataSet();
            sd.Fill(dataSet, tableName);

            _dbConnection.Close();
            return dataSet;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    private bool ConnetTest()
    {
        string connectStr = $"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pwd};";


        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectStr))
            {
                _dbConnection = conn;
                conn.Open();
            }

            Debug.Log("DB 연결을 성공했습니다!");
            return true;
        }
        catch
        {
            Debug.Log("DB 연결을 실패했습니다!");
            return false;
        }
    }

    public void SendQuery()
    {
        if (_isConnectTestCompolete == false)
        {
            Debug.Log("DB 연결을 먼저 시도해주세요.");
            return;
        }
        string query = "SELECT * FROM castledata;";
        SendQuery(query, "castledata");

    }
}
