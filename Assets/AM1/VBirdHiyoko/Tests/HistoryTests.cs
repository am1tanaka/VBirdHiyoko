using AM1.VBirdHiyoko;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class HistoryTests
{
    [SetUp]
    public void SetUp()
    {
        StaticInitializer.Init();
        var objs = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in objs)
        {
            GameObject.DestroyImmediate(obj);
        }
    }

    [TearDown]
    public void TearDown()
    {
        SetUp();
    }

    /// <summary>
    /// メモリを使った履歴のテスト
    /// </summary>
    [Test]
    public void HistoryMemoryStorageTests()
    {
        var mem = new HistoryMemoryStorage();
        HistoryRecorder.historySaver = mem;
        HistoryRecorder.historyLoader = mem;

        // 保存
        var objs = new HistoryBehaviour[4];
        for (int i = 0; i < objs.Length; i++)
        {
            objs[i] = new GameObject("HistoryObject" + i).AddComponent<HistoryBehaviour>();
        }
        objs[0].transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        objs[1].transform.SetPositionAndRotation(new Vector3(1, 0, 0), Quaternion.Euler(0, 90, 0));
        objs[2].transform.SetPositionAndRotation(new Vector3(2, 0, 0), Quaternion.Euler(0, 180, 0));
        objs[3].transform.SetPositionAndRotation(new Vector3(3, 0, 0), Quaternion.Euler(0, 270, 0));

        HistoryObjectRegistrant.RegisterObjects();
        Assert.That(HistoryObjectList.Count, Is.EqualTo(4), "登録数");

        // 移動前確認
        HistoryData result = new();
        objs[0].StartMove();
        objs[1].StartMove();
        objs[2].StartMove();
        objs[3].StartMove();

        objs[0].transform.SetPositionAndRotation(new Vector3(1, 0, 0), Quaternion.Euler(0, 90, 0));
        objs[1].transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
        objs[2].transform.SetPositionAndRotation(new Vector3(2, 0, 1), Quaternion.Euler(0, 0, 0));
        objs[3].transform.SetPositionAndRotation(new Vector3(3, 0, -1), Quaternion.Euler(0, 180, 0));
        HistoryRecorder.MoveDone();

        // それぞれ移動
        objs[0].StartMove();
        objs[1].StartMove();
        objs[2].StartMove();
        objs[3].StartMove();
        objs[0].transform.eulerAngles = new Vector3(0, 0, 0);
        objs[0].transform.Translate(0, 0, 1);
        objs[1].transform.eulerAngles = new Vector3(0, 180, 0);
        objs[1].transform.Translate(0, 0, 1);
        objs[2].transform.eulerAngles = new Vector3(0, 90, 0);
        objs[2].transform.Translate(0, 0, 1);
        objs[3].transform.eulerAngles = new Vector3(0, -90, 0);
        objs[3].transform.Translate(0, 0, 1);
        HistoryRecorder.MoveDone();

        // 保存
        HistoryRecorder.Save(1, 2);

        // 初期化
        HistoryRecorder.Init();
        Assert.That(HistoryObjectList.Count, Is.EqualTo(0), "登録数クリア");
        var latest = HistoryRecorder.UpdateToLatestArray();
        Assert.That(latest.Count, Is.EqualTo(0), "履歴数クリア");
        Assert.That(HistoryRecorder.Counter, Is.Zero, "カウンタークリア");

        // 読み込み
        ushort innerStep;
        HistoryRecorder.Load(1, out innerStep);
        Assert.That(innerStep, Is.EqualTo(2), "内部歩数");
        var history = HistoryRecorder.HistoryArray;
        Assert.That(history[0].step, Is.EqualTo(1), "step1");
        Assert.That(history[0].ObjectId, Is.EqualTo(0), "obj0");
        Assert.That(history[0].RelativePosition, Is.EqualTo(new Vector3Int(1,0,0)), "pos");
        Assert.That(history[0].EulerY, Is.EqualTo(1), "dir");
        Assert.That(history[1].step, Is.EqualTo(1), "step1");
        Assert.That(history[1].ObjectId, Is.EqualTo(1), "obj1");
        Assert.That(history[1].RelativePosition, Is.EqualTo(new Vector3Int(-1, 0, 0)), "pos");
        Assert.That(history[1].EulerY, Is.EqualTo(2), "dir");
        Assert.That(history[2].step, Is.EqualTo(1), "step1");
        Assert.That(history[2].ObjectId, Is.EqualTo(2), "obj2");
        Assert.That(history[2].RelativePosition, Is.EqualTo(new Vector3Int(0, 0, 1)), "pos");
        Assert.That(history[2].EulerY, Is.EqualTo(2), "dir");
        Assert.That(history[3].step, Is.EqualTo(1), "step1");
        Assert.That(history[3].ObjectId, Is.EqualTo(3), "obj3");
        Assert.That(history[3].RelativePosition, Is.EqualTo(new Vector3Int(0, 0, -1)), "pos");
        Assert.That(history[3].EulerY, Is.EqualTo(3), "dir");

        latest = HistoryRecorder.ToLatestArray;
        Assert.That(latest.Count, Is.EqualTo(4), "履歴数読み込み");

        Assert.That(latest[0].ObjectId, Is.EqualTo(0), "obj0");
        Assert.That(latest[0].RelativePosition, Is.EqualTo(new Vector3Int(1, 0, 1)), "pos");
        Assert.That(latest[0].EulerY, Is.EqualTo(0), "dir");
        Assert.That(latest[1].ObjectId, Is.EqualTo(1), "obj1");
        Assert.That(latest[1].RelativePosition, Is.EqualTo(new Vector3Int(-1, 0, -1)), "pos");
        Assert.That(latest[1].EulerY, Is.EqualTo(1), "dir");
        Assert.That(latest[2].ObjectId, Is.EqualTo(2), "obj2");
        Assert.That(latest[2].RelativePosition, Is.EqualTo(new Vector3Int(1, 0, 1)), "pos");
        Assert.That(latest[2].EulerY, Is.EqualTo(3), "dir");
        Assert.That(latest[3].ObjectId, Is.EqualTo(3), "obj3");
        Assert.That(latest[3].RelativePosition, Is.EqualTo(new Vector3Int(-1, 0, -1)), "pos");
        Assert.That(latest[3].EulerY, Is.EqualTo(0), "dir");

        Assert.That(HistoryRecorder.Counter, Is.EqualTo(2), "カウンター2");

        DestroyObjects(objs);
    }

    /// <summary>
    /// HistoryRecorderとのHistoryBehaviourの組み合わせテスト
    /// </summary>
    [Test]
    public void HistoryRecorderTests()
    {
        var objs = new HistoryBehaviour[4];
        for (int i = 0; i < objs.Length; i++)
        {
            objs[i] = new GameObject("HistoryObject" + i).AddComponent<HistoryBehaviour>();
        }
        objs[0].transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        objs[1].transform.SetPositionAndRotation(new Vector3(1, 0, 0), Quaternion.Euler(0, 90, 0));
        objs[2].transform.SetPositionAndRotation(new Vector3(2, 0, 0), Quaternion.Euler(0, 180, 0));
        objs[3].transform.SetPositionAndRotation(new Vector3(3, 0, 0), Quaternion.Euler(0, 270, 0));

        HistoryObjectRegistrant.RegisterObjects();
        Assert.That(HistoryObjectList.Count, Is.EqualTo(4), "登録数");

        // 移動前確認
        HistoryData result = new();
        objs[0].StartMove();
        objs[1].StartMove();
        objs[2].StartMove();
        objs[3].StartMove();

        objs[0].transform.SetPositionAndRotation(new Vector3(1, 0, 0), Quaternion.Euler(0, 90, 0));
        objs[1].transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
        objs[2].transform.SetPositionAndRotation(new Vector3(2, 0, 1), Quaternion.Euler(0, 0, 0));
        objs[3].transform.SetPositionAndRotation(new Vector3(3, 0, -1), Quaternion.Euler(0, 180, 0));
        HistoryRecorder.MoveDone();

        var array = HistoryRecorder.HistoryArray;
        var sbytes = array[0].GetSBytes();
        Assert.That(array.Count, Is.EqualTo(4), "履歴数");
        Assert.That(sbytes[0], Is.EqualTo(1), "一歩");
        Assert.That(sbytes[1], Is.EqualTo(0), "一歩");
        Assert.That(sbytes[2], Is.EqualTo(1), "一歩");
        Assert.That(sbytes[3], Is.EqualTo(0), "一歩");
        Assert.That(sbytes[4], Is.EqualTo(0), "一歩");
        Assert.That(sbytes[5], Is.EqualTo(1), "一歩");
        sbytes = array[1].GetSBytes();
        Assert.That(sbytes[0], Is.EqualTo(1), "一歩");
        Assert.That(sbytes[1], Is.EqualTo(1), "一歩");
        Assert.That(sbytes[2], Is.EqualTo(-1), "一歩");
        Assert.That(sbytes[3], Is.EqualTo(0), "一歩");
        Assert.That(sbytes[4], Is.EqualTo(0), "一歩");
        Assert.That(sbytes[5], Is.EqualTo(2), "一歩");
        sbytes = array[2].GetSBytes();
        Assert.That(sbytes[0], Is.EqualTo(1), "一歩");
        Assert.That(sbytes[1], Is.EqualTo(2), "一歩");
        Assert.That(sbytes[2], Is.EqualTo(0), "一歩");
        Assert.That(sbytes[3], Is.EqualTo(0), "一歩");
        Assert.That(sbytes[4], Is.EqualTo(1), "一歩");
        Assert.That(sbytes[5], Is.EqualTo(2), "一歩");
        sbytes = array[3].GetSBytes();
        Assert.That(sbytes[0], Is.EqualTo(1), "一歩");
        Assert.That(sbytes[1], Is.EqualTo(3), "一歩");
        Assert.That(sbytes[2], Is.EqualTo(0), "一歩");
        Assert.That(sbytes[3], Is.EqualTo(0), "一歩");
        Assert.That(sbytes[4], Is.EqualTo(-1), "一歩");
        Assert.That(sbytes[5], Is.EqualTo(3), "一歩");

        // それぞれ移動
        objs[0].StartMove();
        objs[1].StartMove();
        objs[2].StartMove();
        objs[3].StartMove();
        objs[0].transform.eulerAngles = new Vector3(0, 0, 0);
        objs[0].transform.Translate(0, 0, 1);
        objs[1].transform.eulerAngles = new Vector3(0, 180, 0);
        objs[1].transform.Translate(0, 0, 1);
        objs[2].transform.eulerAngles = new Vector3(0, 90, 0);
        objs[2].transform.Translate(0, 0, 1);
        objs[3].transform.eulerAngles = new Vector3(0, -90, 0);
        objs[3].transform.Translate(0, 0, 1);
        HistoryRecorder.MoveDone();
        array = HistoryRecorder.HistoryArray;
        int index = HistoryRecorder.GetStepIndex(2);
        Assert.That(index, Is.EqualTo(4), "2番目");
        sbytes = array[4].GetSBytes();
        Assert.That(array.Count, Is.EqualTo(8), "履歴数");
        Assert.That(sbytes[0], Is.EqualTo(2), "それぞれ移動");
        Assert.That(sbytes[1], Is.EqualTo(0), "それぞれ移動");
        Assert.That(sbytes[2], Is.EqualTo(0), "それぞれ移動");
        Assert.That(sbytes[3], Is.EqualTo(0), "それぞれ移動");
        Assert.That(sbytes[4], Is.EqualTo(1), "それぞれ移動");
        Assert.That(sbytes[5], Is.EqualTo(3), "それぞれ移動");
        sbytes = array[5].GetSBytes();
        Assert.That(sbytes[0], Is.EqualTo(2), "それぞれ移動");
        Assert.That(sbytes[1], Is.EqualTo(1), "それぞれ移動");
        Assert.That(sbytes[2], Is.EqualTo(0), "それぞれ移動");
        Assert.That(sbytes[3], Is.EqualTo(0), "それぞれ移動");
        Assert.That(sbytes[4], Is.EqualTo(-1), "それぞれ移動");
        Assert.That(sbytes[5], Is.EqualTo(3), "それぞれ移動");
        sbytes = array[6].GetSBytes();
        Assert.That(sbytes[0], Is.EqualTo(2), "それぞれ移動");
        Assert.That(sbytes[1], Is.EqualTo(2), "それぞれ移動");
        Assert.That(sbytes[2], Is.EqualTo(1), "それぞれ移動");
        Assert.That(sbytes[3], Is.EqualTo(0), "それぞれ移動");
        Assert.That(sbytes[4], Is.EqualTo(0), "それぞれ移動");
        Assert.That(sbytes[5], Is.EqualTo(1), "それぞれ移動");
        sbytes = array[7].GetSBytes();
        Assert.That(sbytes[0], Is.EqualTo(2), "それぞれ移動");
        Assert.That(sbytes[1], Is.EqualTo(3), "それぞれ移動");
        Assert.That(sbytes[2], Is.EqualTo(-1), "それぞれ移動");
        Assert.That(sbytes[3], Is.EqualTo(0), "それぞれ移動");
        Assert.That(sbytes[4], Is.EqualTo(0), "それぞれ移動");
        Assert.That(sbytes[5], Is.EqualTo(1), "それぞれ移動");

        // スタート位置からの移動量
        var toLatest = HistoryRecorder.UpdateToLatestArray();
        //// データ解放と再生成のチェックのため、もう一度実行
        toLatest = HistoryRecorder.UpdateToLatestArray();
        Assert.That(toLatest.Length, Is.EqualTo(4), "スタート位置からの移動量");        
        // 1, 0, 1
        sbytes = toLatest[0].GetSBytes();
        Assert.That(sbytes[1], Is.EqualTo(0), "移動量1");
        Assert.That(sbytes[2], Is.EqualTo(1), "移動量1");
        Assert.That(sbytes[3], Is.EqualTo(0), "移動量1");
        Assert.That(sbytes[4], Is.EqualTo(1), "移動量1");
        // -1, 0, -1
        sbytes = toLatest[1].GetSBytes();
        Assert.That(sbytes[1], Is.EqualTo(1), "移動量1");
        Assert.That(sbytes[2], Is.EqualTo(-1), "移動量1");
        Assert.That(sbytes[3], Is.EqualTo(0), "移動量1");
        Assert.That(sbytes[4], Is.EqualTo(-1), "移動量1");
        // 1, 0, 1
        sbytes = toLatest[2].GetSBytes();
        Assert.That(sbytes[1], Is.EqualTo(2), "移動量1");
        Assert.That(sbytes[2], Is.EqualTo(1), "移動量1");
        Assert.That(sbytes[3], Is.EqualTo(0), "移動量1");
        Assert.That(sbytes[4], Is.EqualTo(1), "移動量1");
        // -1, 0, -1
        sbytes = toLatest[3].GetSBytes();
        Assert.That(sbytes[1], Is.EqualTo(3), "移動量1");
        Assert.That(sbytes[2], Is.EqualTo(-1), "移動量1");
        Assert.That(sbytes[3], Is.EqualTo(0), "移動量1");
        Assert.That(sbytes[4], Is.EqualTo(-1), "移動量1");

        DestroyObjects(objs);
    }


    /// <summary>
    /// HistoryBehaviour単体での動作テスト
    /// </summary>
    [Test]
    public void HistoryBehaviourTests()
    {
        var historyObjects = new HistoryBehaviour[1];
        for (int i = 0; i < historyObjects.Length; i++)
        {
            historyObjects[i] = new GameObject("HistoryObject" + i).AddComponent<HistoryBehaviour>();
        }
        historyObjects[0].transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

        HistoryObjectRegistrant.RegisterObjects();
        Assert.That(HistoryObjectList.Count, Is.EqualTo(1), "登録数");

        // 移動前確認
        HistoryData result = new();
        historyObjects[0].StartMove();
        Assert.That(historyObjects[0].MoveDone(ref result), Is.False, "移動してない");
        Assert.That(historyObjects[0].ToLatestData(ref result), Is.False, "移動してない");

        // 奥へ2歩
        historyObjects[0].StartMove();
        historyObjects[0].transform.Translate(2 * Vector3.forward);
        Assert.That(historyObjects[0].MoveDone(ref result), Is.True, "奥へ2歩");
        var bytes = result.GetSBytes();
        Assert.That(bytes[(int)HistoryData.Type.ObjectId], Is.EqualTo(0), "オブジェクトID");
        Assert.That(bytes[(int)HistoryData.Type.DX], Is.Zero, "奥");
        Assert.That(bytes[(int)HistoryData.Type.DY], Is.Zero, "奥");
        Assert.That(bytes[(int)HistoryData.Type.DZ], Is.EqualTo(2), "奥");
        Assert.That(bytes[(int)HistoryData.Type.EulerYAndState], Is.Zero, "奥");

        // 右へ一歩で右向き
        historyObjects[0].StartMove();
        historyObjects[0].transform.SetPositionAndRotation(new Vector3(1, 0, 2), Quaternion.Euler(0, 90, 0));
        Assert.That(historyObjects[0].MoveDone(ref result), Is.True, "右へ一歩、右向き");
        bytes = result.GetSBytes();
        Assert.That(bytes[(int)HistoryData.Type.ObjectId], Is.EqualTo(0), "右へ一歩、右向き");
        Assert.That(bytes[(int)HistoryData.Type.DX], Is.EqualTo(1), "右へ一歩、右向き");
        Assert.That(bytes[(int)HistoryData.Type.DY], Is.Zero, "右へ一歩、右向き");
        Assert.That(bytes[(int)HistoryData.Type.DZ], Is.Zero, "右へ一歩、右向き");
        Assert.That(bytes[(int)HistoryData.Type.EulerYAndState], Is.EqualTo(1), "右へ一歩、右向き");

        // 手前へ一歩して手前を向いてstateを1
        string mes = "手前へ一歩して手前を向いてstateを1";
        historyObjects[0].StartMove();
        historyObjects[0].transform.SetPositionAndRotation(new Vector3(1, 0, 1), Quaternion.Euler(0, 180, 0));
        historyObjects[0].State = 1;
        Assert.That(historyObjects[0].MoveDone(ref result), Is.True, mes);
        bytes = result.GetSBytes();
        Assert.That(bytes[(int)HistoryData.Type.ObjectId], Is.EqualTo(0), mes);
        Assert.That(bytes[(int)HistoryData.Type.DX], Is.EqualTo(0), mes);
        Assert.That(bytes[(int)HistoryData.Type.DY], Is.EqualTo(0), mes);
        Assert.That(bytes[(int)HistoryData.Type.DZ], Is.EqualTo(-1), mes);
        Assert.That(bytes[(int)HistoryData.Type.EulerYAndState], Is.EqualTo(1 + 4), mes);

        // 左へ2歩
        mes = "左へ2歩";
        historyObjects[0].StartMove();
        historyObjects[0].transform.SetPositionAndRotation(new Vector3(-1, 0, 1), Quaternion.Euler(0, 270, 0));
        Assert.That(historyObjects[0].MoveDone(ref result), Is.True, mes);
        bytes = result.GetSBytes();
        Assert.That(bytes[(int)HistoryData.Type.ObjectId], Is.EqualTo(0), mes);
        Assert.That(bytes[(int)HistoryData.Type.DX], Is.EqualTo(-2), mes);
        Assert.That(bytes[(int)HistoryData.Type.DY], Is.EqualTo(0), mes);
        Assert.That(bytes[(int)HistoryData.Type.DZ], Is.EqualTo(0), mes);
        Assert.That(bytes[(int)HistoryData.Type.EulerYAndState], Is.EqualTo(1), mes);

        // スタートから最終
        mes = "スタートから最終";
        Assert.That(historyObjects[0].ToLatestData(ref result), Is.True, "スタートから");
        bytes = result.GetSBytes();
        Assert.That(bytes[(int)HistoryData.Type.ObjectId], Is.EqualTo(0), mes);
        Assert.That(bytes[(int)HistoryData.Type.DX], Is.EqualTo(-1), mes);
        Assert.That(bytes[(int)HistoryData.Type.DY], Is.EqualTo(0), mes);
        Assert.That(bytes[(int)HistoryData.Type.DZ], Is.EqualTo(1), mes);
        Assert.That(bytes[(int)HistoryData.Type.EulerYAndState], Is.EqualTo(3 + 4), mes);

        DestroyObjects(historyObjects);
    }

    [Test]
    public void RotateTests()
    {
        Transform transform = new GameObject("RotateTest").transform;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Assert.That(HistoryBehaviour.EulerYToDir(transform.eulerAngles.y), Is.EqualTo(0), $"RotateYToDir({transform.eulerAngles.y})");
        transform.Rotate(0, 90, 0);
        Assert.That(HistoryBehaviour.EulerYToDir(transform.eulerAngles.y), Is.EqualTo(1), $"RotateYToDir({transform.eulerAngles.y})");
        transform.Rotate(0, 90, 0);
        Assert.That(HistoryBehaviour.EulerYToDir(transform.eulerAngles.y), Is.EqualTo(2), $"RotateYToDir({transform.eulerAngles.y})");
        transform.Rotate(0, 90, 0);
        Assert.That(HistoryBehaviour.EulerYToDir(transform.eulerAngles.y), Is.EqualTo(3), $"RotateYToDir({transform.eulerAngles.y})");
        transform.Rotate(0, 90, 0);
        Assert.That(HistoryBehaviour.EulerYToDir(transform.eulerAngles.y), Is.EqualTo(0), $"RotateYToDir({transform.eulerAngles.y})");

        // 逆回転
        transform.Rotate(0, -90, 0);
        Assert.That(HistoryBehaviour.EulerYToDir(transform.eulerAngles.y), Is.EqualTo(3), $"RotateYToDir({transform.eulerAngles.y})");
        transform.Rotate(0, -90, 0);
        Assert.That(HistoryBehaviour.EulerYToDir(transform.eulerAngles.y), Is.EqualTo(2), $"RotateYToDir({transform.eulerAngles.y})");
        transform.Rotate(0, -90, 0);
        Assert.That(HistoryBehaviour.EulerYToDir(transform.eulerAngles.y), Is.EqualTo(1), $"RotateYToDir({transform.eulerAngles.y})");
        transform.Rotate(0, -90, 0);
        Assert.That(HistoryBehaviour.EulerYToDir(transform.eulerAngles.y), Is.EqualTo(0), $"RotateYToDir({transform.eulerAngles.y})");

        transform.rotation = Quaternion.Euler(0, 360 + 90, 0);
        Assert.That(HistoryBehaviour.EulerYToDir(transform.eulerAngles.y), Is.EqualTo(1), $"RotateYToDir({transform.eulerAngles.y})");

        GameObject.Destroy(transform.gameObject);
    }

    [Test]
    public void HistoryFundamentalUnitTests()
    {
        var historyObjects = new HistoryBehaviour[4];
        for (int i = 0; i < historyObjects.Length; i++)
        {
            historyObjects[i] = new GameObject("HistoryObject" + i).AddComponent<HistoryBehaviour>();
        }
        historyObjects[0].transform.position = new Vector3(0, 0, 0);
        historyObjects[1].transform.position = new Vector3(2, 0, 0);
        historyObjects[2].transform.position = new Vector3(1, 0, 0);
        historyObjects[3].transform.position = new Vector3(3, 0, 0);

        Assert.That(HistoryObjectList.Count, Is.EqualTo(0), "登録なし");

        HistoryObjectRegistrant.RegisterObjects();

        Assert.That(HistoryObjectList.Count, Is.EqualTo(4), "登録数");

        // 範囲の確認
        Assert.That(HistoryObjectRegistrant.MapBounds.max.x, Is.EqualTo(3).Within(0.1f), "範囲の確認");

        // Idの確認
        Assert.That(historyObjects[0].Id, Is.EqualTo(0), "Idの確認");
        Assert.That(historyObjects[1].Id, Is.EqualTo(2), "Idの確認");
        Assert.That(historyObjects[2].Id, Is.EqualTo(1), "Idの確認");
        Assert.That(historyObjects[3].Id, Is.EqualTo(3), "Idの確認");

        DestroyObjects(historyObjects);
    }

    void DestroyObjects(HistoryBehaviour[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject.DestroyImmediate(objects[i]);
        }
    }
}
