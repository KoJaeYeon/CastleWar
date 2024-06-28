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
    Both,
    Skill
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
    public UnitType unitType;
    public float health;
    public float AttackDamage;
    public float AttackSpeed;
    public float AttackRange;
    public int MoveSpeed;
    public UnitType AttackType;
    public int Population;
    public CardType cardType;

    public UnitData(int id, string name, int cost, UnitType unitType, float health, float attackDamage, float attackSpeed, float attackRange, int moveSpeed, UnitType attackType, int population, CardType cardType)
    {
        this.id = id;
        this.name = name;
        this.cost = cost;
        this.unitType = unitType;
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

    public UnitData OnGetUnitData(int id)
    {
        return unitDataDic[id];
    }
    public Sprite OnGetSpriteData(int id)
    {
        return unitSpriteDic[id];
    }

    private void Awake()
    {
        if (_instance == null || _instance == this)
        {
            _instance = this;
        }
        else
        {
            Debug.Log("Destroy");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        ConnectToDB();
    }

    public void ConnectToDB()
    {
        //[TODO : 최종 빌드때 제거]
        //_isConnectTestCompolete = ConnetTest();
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
                    if(id < 6)
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
            Debug.Log("DB 연결에 실패했습니다. 수동으로 데이터를 읽어옵니다.");
            ReadDataManually();
            return;
        }
        string query = "SELECT * FROM castledata;";
        SendQuery(query, "castledata");

    }

    public void ReadDataManually()
    {
        unitDataDic.Clear();

        // 데이터를 추가
        unitDataDic.Add(-1, new UnitData(0, "막사", 50, UnitType.Building, 2750, 0, 0, 0, 0, UnitType.Ground, 10, CardType.Common));
        unitDataDic.Add(0, new UnitData(0, "궁수탑", 50, UnitType.Building, 1650, 45, 0.4f, 7, 0, UnitType.Both, 5, CardType.Common));
        unitDataDic.Add(1, new UnitData(1, "포탑", 50, UnitType.Building, 1650, 296, 1.7f, 6, 0, UnitType.Ground, 5, CardType.Common));
        unitDataDic.Add(2, new UnitData(2, "대공포탑", 50, UnitType.Building, 1650, 130, 0.8f, 9, 0, UnitType.Air, 5, CardType.Common));
        unitDataDic.Add(3, new UnitData(3, "코볼트", 8, UnitType.Ground, 120, 52, 0.7f, 3, 7, UnitType.Ground, 1, CardType.Common));
        unitDataDic.Add(4, new UnitData(4, "전열보병", 25, UnitType.Ground, 730, 106, 1.3f, 3, 7, UnitType.Ground, 3, CardType.Common));
        unitDataDic.Add(5, new UnitData(5, "헌터", 15, UnitType.Ground, 195, 45, 0.8f, 5, 5, UnitType.Both, 2, CardType.Common));
        unitDataDic.Add(6, new UnitData(6, "트릭스터", 18, UnitType.Ground, 366, 45, 0.9f, 2, 5, UnitType.Both, 2, CardType.Common));
        unitDataDic.Add(7, new UnitData(7, "방패 펭귄병", 15, UnitType.Ground, 400, 52, 1.5f, 3, 5, UnitType.Ground, 2, CardType.Common));
        unitDataDic.Add(8, new UnitData(8, "리자드 투창병", 15, UnitType.Ground, 240, 106, 2.6f, 6, 5, UnitType.Ground, 2, CardType.Common));
        unitDataDic.Add(9, new UnitData(9, "꼬마 바이킹", 25, UnitType.Ground, 1288, 256, 2.2f, 3, 7, UnitType.Building, 3, CardType.Epic));
        unitDataDic.Add(10, new UnitData(10, "마나 캐논", 75, UnitType.Building, 3660, 685, 2.8f, 8, 5, UnitType.Ground, 6, CardType.Rare));
        unitDataDic.Add(11, new UnitData(11, "발리스타", 75, UnitType.Building, 3660, 1021, 2.6f, 7, 5, UnitType.Ground, 6, CardType.Rare));
        unitDataDic.Add(12, new UnitData(12, "글레이셔", 40, UnitType.Ground, 926, 259, 2.0f, 5, 5, UnitType.Both, 4, CardType.Legendary));
        unitDataDic.Add(13, new UnitData(13, "해칠링", 40, UnitType.Air, 133, 121, 1.2f, 3, 7, UnitType.Both, 4, CardType.Rare));
        unitDataDic.Add(14, new UnitData(14, "그리즐리", 60, UnitType.Ground, 3594, 253, 1.5f, 3, 3, UnitType.Building, 5, CardType.Rare));
        unitDataDic.Add(15, new UnitData(15, "요술사", 48, UnitType.Ground, 1048, 511, 2.0f, 5, 5, UnitType.Both, 4, CardType.Rare));
        unitDataDic.Add(16, new UnitData(16, "스매셔", 50, UnitType.Ground, 1617, 572, 2.1f, 3, 5, UnitType.Ground, 5, CardType.Rare));
        unitDataDic.Add(17, new UnitData(17, "용인", 50, UnitType.Ground, 1487, 201, 1.1f, 3, 5, UnitType.Ground, 5, CardType.Rare));
        unitDataDic.Add(18, new UnitData(18, "소총 아울베어", 45, UnitType.Ground, 925, 237, 2.3f, 7, 5, UnitType.Both, 4, CardType.Rare));
        unitDataDic.Add(19, new UnitData(19, "모스레이디", 40, UnitType.Air, 938, 180, 1.6f, 2, 7, UnitType.Both, 4, CardType.Rare));
        unitDataDic.Add(20, new UnitData(20, "전투 코끼리", 50, UnitType.Ground, 2641, 237, 1.4f, 7, 7, UnitType.Building, 5, CardType.Rare));
        unitDataDic.Add(21, new UnitData(21, "인터셉터", 35, UnitType.Air, 532, 157, 1.4f, 2, 7, UnitType.Ground, 3, CardType.Rare));
        unitDataDic.Add(22, new UnitData(22, "전투 고릴라", 45, UnitType.Ground, 1158, 615, 2.8f, 10, 3, UnitType.Building, 5, CardType.Rare));
        unitDataDic.Add(23, new UnitData(23, "샐러맨더", 50, UnitType.Ground, 1138, 526, 2.3f, 6, 3, UnitType.Ground, 5, CardType.Rare));
        unitDataDic.Add(24, new UnitData(24, "순교자", 40, UnitType.Ground, 1019, 243, 1.5f, 4, 5, UnitType.Both, 4, CardType.Epic));
        unitDataDic.Add(25, new UnitData(25, "비행선", 35, UnitType.Air, 2407, 195, 3.0f, 1, 7, UnitType.Building, 3, CardType.Legendary));
        unitDataDic.Add(26, new UnitData(26, "초월의 탑", 150, UnitType.Building, 7015, 5456, 1.0f, 99, 7, UnitType.Both, 8, CardType.Legendary));
        unitDataDic.Add(27, new UnitData(27, "아케인 블래스터", 100, UnitType.Building, 6442, 324, 4.0f, 7, 5, UnitType.Both, 7, CardType.Epic));
        unitDataDic.Add(28, new UnitData(28, "그리폰 라이더", 70, UnitType.Air, 1683, 354, 1.5f, 8, 5, UnitType.Ground, 5, CardType.Legendary));
        unitDataDic.Add(29, new UnitData(29, "대포 거북", 85, UnitType.Ground, 1772, 672, 2.9f, 8, 7, UnitType.Ground, 6, CardType.Epic));
        unitDataDic.Add(30, new UnitData(30, "강철 거인", 100, UnitType.Ground, 7972, 1047, 2.4f, 3, 3, UnitType.Building, 8, CardType.Legendary));
        unitDataDic.Add(31, new UnitData(31, "드래곤", 100, UnitType.Air, 5047, 448, 2.3f, 4, 3, UnitType.Both, 8, CardType.Epic));
        unitDataDic.Add(32, new UnitData(32, "대공포병", 80, UnitType.Ground, 2206, 934, 2.1f, 6, 7, UnitType.Both, 6, CardType.Epic));
        unitDataDic.Add(33, new UnitData(33, "엑스 버팔로", 90, UnitType.Ground, 3704, 1517, 2.7f, 3, 3, UnitType.Ground, 7, CardType.Legendary));
        unitDataDic.Add(34, new UnitData(34, "이블리스", 80, UnitType.Air, 3293, 522, 4.0f, 5, 3, UnitType.Both, 6, CardType.Legendary));
        unitDataDic.Add(35, new UnitData(35, "사령여제", 70, UnitType.Ground, 1890, 495, 1.3f, 6, 7, UnitType.Both, 5, CardType.Legendary));
        unitDataDic.Add(36, new UnitData(36, "열기구 펭귄병", 40, UnitType.Air, 720, 128, 2.2f, 2, 5, UnitType.Both, 4, CardType.Rare));
        unitDataDic.Add(37, new UnitData(37, "저격수", 100, UnitType.Ground, 1198, 1091, 2.9f, 10, 3, UnitType.Both, 6, CardType.Legendary));
        unitDataDic.Add(38, new UnitData(38, "켄타우루스", 80, UnitType.Ground, 2738, 730, 1.6f, 3, 7, UnitType.Ground, 6, CardType.Epic));

        for (int id = 0; id < 6; id++)
        {

            Sprite unitSprite = Resources.Load<Sprite>($"Sprites/UI/UnitSprite/Unit_{id}") as Sprite;
            unitSpriteDic.Add(id, unitSprite);

        }
    }


}
