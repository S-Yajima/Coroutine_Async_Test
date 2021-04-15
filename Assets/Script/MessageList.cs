using System.Collections.Generic;

public class Message {
    private string m_message = "";
    private string m_code = "";

    public Message(string message)
    {
        m_message = message;
    }

    public Message (string message, string code)
    {
        m_message = message;
        m_code = code;
    }

    public string getMessage()
    {
        return m_message;
    }

    public string getCode()
    {
        return m_code;
    }
}


public class MessageList {

    private List<Message> messageList = new List<Message>();

    public MessageList()
    {
        string code_message_1 = "【下記(1)(2)を繰り返しています】\n\n(1) 一定の時間だけ待機\n\n(2) m_sphere_Coroutine.AddForce(m_vector); // オブジェクトを浮かせる";
        string code_message_2 = "[SerializeField] private float m_interval = 3f;\nprivate float m_deltaTime = 0f;\n\nvoid Update() {\nm_deltaTime += Time.deltaTime;\n  <#00FFFF>if (m_deltaTime >= m_interval)        {  // deltaTimeの判別により待機</color>\n    m_deltaTime = 0f;\n    <#00FFFF>m_sphere_deltaTime.AddForce(m_vector);   // オブジェクトを浮かせる</color>\n  }";
        string code_message_3 = "void InvokeTest() {\n  m_Invoke_flg = true;\n}\n\nvoid Update() {\n  if (m_Invoke_flg == true) {\n    m_Invoke_flg = false;\n    <#00FFFF>m_sphere_Invoke.AddForce(m_vector);  // オブジェクトを浮かせる </color>\n    <#00FFFF>Invoke(\"InvokeTest\", m_interval);   // 待機時間を指定しながら呼び出しを行う</color>\n  }\n";
        string code_message_4 = "IEnumerator CoroutineTest() {\n  <#00FFFF>yield return new WaitForSeconds(m_interval);   // ここで待機を行う</color>\n  m_Coroutine_flg = true;\n  yield return 0;\n}\n\nvoid Update() {\n  if (m_Coroutine_flg == true) {\n    m_Coroutine_flg = false;\n    <#00FFFF>m_sphere_Coroutine.AddForce(m_vector);    // オブジェクトを浮かせる</color>\n    StartCoroutine(CoroutineTest());\n  }";
        string code_message_5 = "private async Task AwaitTest() {\n  <#00FFFF>await Task.Delay((int)m_interval * 1000);   // ここで待機を行う</color>\n  m_AsyncAwait_flg = true;\n}\n\nasync Task AsyncTest() {\n  await AwaitTest();\n}\n\nvoid Update() {\n  if (m_AsyncAwait_flg == true) {\n    m_AsyncAwait_flg = false;\n    <#00FFFF>m_sphere_AsyncAwait.AddForce(m_vector);    // オブジェクトを浮かせる</color>\n    AsyncTest();\n  }";
        string code_message_6 = "private async Task AwaitTest() {\n  <#FFFF00>await Task.Delay((int)m_interval * 1000);</color>\n  m_AsyncAwait_flg = true;\n}\nasync Task AsyncTest() {\n  await AwaitTest();\n}\nvoid Update() {\n  if (m_AsyncAwait_flg == true) {\n    m_AsyncAwait_flg = false;\n    m_sphere_AsyncAwait.AddForce(m_vector);\n    AsyncTest();\n  }";

        messageList.Add(new Message("繰り返し処理は好きですか？よく書きますか？"));                           // 0
        messageList.Add(new Message("Time.deltaTime、Invoke()、Coroutine、async+await+Task で"));
        messageList.Add(new Message("同じ待機時間 (3秒) を挟んで挙動を繰り返す処理を動かしています。", code_message_1));
        messageList.Add(new Message("繰り返している処理は、球体に上方への力を加えて浮かせる処理です。", code_message_1));
        messageList.Add(new Message("果たしてTime.deltaTime、Invoke()、Coroutine、async+await+Task は", code_message_1));
        messageList.Add(new Message("同じタイミングで繰り返されるのか？実行されるのか？", code_message_1));                          // 5
        messageList.Add(new Message("もしTime.deltaTimeと同様の周期で実行されれば"));
        messageList.Add(new Message("Sceneの挙動やランタイムと関係があるのかもしれません。"));
        messageList.Add(new Message("async + await は早くも他と周期がずれてきていますね。"));
        messageList.Add(new Message("ちなみに Time.deltaTime はこんな感じのコード。", code_message_2));
        messageList.Add(new Message("Invoke() はこんなコード。", code_message_3));                                             // 10
        messageList.Add(new Message("Coroutineはこんなコード。WaitForSeconds()で待機してます。", code_message_4));
        messageList.Add(new Message("async+await はこんなコード。Task.Delay()で待機しています。", code_message_5));
        messageList.Add(new Message("Task.Delay()の他にもTask.Run()のラムダ式の中で", code_message_6));
        messageList.Add(new Message("Thread.Sleep()で待機する方法もありますが今回の試みでは似た結果になります。", code_message_6));
        messageList.Add(new Message("Invoke も少しタイミングがズレてきた感じがしますね。"));
        messageList.Add(new Message("今回の結果から、Time.deltaTime と Coroutine は関係が深く"));
        messageList.Add(new Message("Invoke() や async + await は別の時間軸で動いているかもしれない。"));
        messageList.Add(new Message("と、想像してみました。ではまた！ノシ"));
    }

    public string getMessage(int index)
    {
        if (messageList.Count == 0 || messageList.Count <= index) return "";

        return messageList[index].getMessage();
    }

    public string getCode(int index)
    {
        if (messageList.Count == 0 || messageList.Count <= index) return "";

        return messageList[index].getCode();
    }

    public int count()
    {
        return messageList.Count;
    }
}
