using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using TMPro;


/*
 * ・Time.deltaTime, Invoke, Coroutine, async await + Task にて 一定時間待機 + 繰り返し実行 の実装を試す
 * 　> SceneのPlay を停止しても async + Task にて呼ばれた処理が動作してしまう。
 * 　> Scene と スレッド実行 のランタイム間の連携に問題が見られる。(async Task の処理に対し再帰呼び出しを行うと顕著)
 * 　> Syncronized, .wait(), .lock() などのスレッドセーフ、デッドロック対策は存在しないのか疑問が残る。
 * 　
 * ・GameObjectに子オブジェクトとしてText(TMP)を設定し動作を確認する
 *   > Rect Transform の PosX/Y/Z , Pivot X/Y の設定で親オブジェクトとの位置関係を設定できる
 *   
 * ・await にて呼び出されるメソッドの実装を試みる
 * 
 * ・ScrollViewによる文章表示を試みる
 *   > ScrollView の スライダーは消せる
 *   > ViewPort の Mask が 有効になっていると、消したスライダー領域に文字が隠れてしまう。
 *   
 *   
 */
public class DirectScript : MonoBehaviour
{
    // Scene内 スフィア の Rigidbody
    [SerializeField] private Rigidbody m_sphere_deltaTime;
    [SerializeField] private Rigidbody m_sphere_Invoke;
    [SerializeField] private Rigidbody m_sphere_Coroutine;
    [SerializeField] private Rigidbody m_sphere_AsyncAwait;
    // Text(TMP)
    [SerializeField] private TextMeshProUGUI m_message_text;
    [SerializeField] private TextMeshProUGUI m_code_text;
    // スフィアにAddForce()を行うタイムインターバル
    [SerializeField] private float m_interval = 3f;

    // 繰り返し処理実行タイミングのフラグ
    private bool m_Invoke_flg = false;
    private bool m_Coroutine_flg = false;
    private bool m_AsyncAwait_flg = false;

    // スフィアに与えるAddForceのベクトル
    private Vector3 m_vector = new Vector3(0f, 250f, 0f);
    // deltaTimeを計測するためのメンバ変数
    private float m_deltaTime = 0f;

    // メッセージList
    private MessageList m_messages = new MessageList();
    // メッセージ表示のインターバルとListインデックス
    private float m_message_interval = 5f;
    private int m_message_index = 0;



    // メッセージを更新する
    // 再帰呼び出しにて繰り返す。(Coroutine内での再帰呼び出しに弊害がないかは不明)
    // 表示対象のメッセージがなくなったら処理を抜ける。
    IEnumerator changeMessage()
    {
        yield return new WaitForSeconds(m_message_interval);

        m_message_text.text = m_messages.getMessage(m_message_index);
        m_code_text.text = m_messages.getCode(m_message_index);
        m_message_index++;

        if (m_message_index < m_messages.count())
        {
            StartCoroutine(changeMessage());
        }
        yield return 0;
    }


    // コルーチンにてフラグを更新する
    IEnumerator CoroutineTest()
    {
        yield return new WaitForSeconds(m_interval);
        m_Coroutine_flg = true;
        yield return 0;
    }


    // await で呼び出されるメソッドにてフラグ更新を実行する
    // 呼び出しもとでフラグを変更して問題ないが、
    // awaitで呼び出せるメソッドの作成を試すため記述したメソッド
    private async Task AwaitTest()
    {
        await Task.Delay((int)m_interval * 1000);
        //Thread.Sleep((int)m_interval * 1000); // <- これだと不具合。エラーは出力されないが想定外の動きをしてしまう。
                                                // Thread.Sleep() を呼び出すときは明示的にTask.Run(() => ラムダ式);
                                                // として、別スレッドとして実行する。
        m_AsyncAwait_flg = true;

        // Task<int>, Task<float>,  などの場合は return 数値;
        // Task<string> の場合は return "xxxx";
    }


    // async await + Task にてフラグを更新する
    //
    // Scene の Play を中止しても、タイミングにより動作してしまうケースがある模様。作りを再帰呼び出しにすると確実に再現する。
    // async await + Task の 実行ランタイム と Scene の 実行ランタイム の関係性には問題が多いと推測する。
    // マルチスレッドの処理を async await といった簡潔すぎる記述で書ける事自体違和感。
    // syncronized, .lock(), .wait() と言った、デッドロック防止や情報の原子性を確保する仕組みの記述は考慮しなくて良いのか疑問。
    // 上記問題解決のために UniRx/UniTask と呼ばれるライブラリが作られたと推測する。
    async Task AsyncTest()
    {
        await AwaitTest();

        //await Task.Delay((int)m_interval * 1000);
        //m_AsyncAwait_flg = true;

        /*
        await Task.Run(() => {
            Thread.Sleep((int)m_interval * 1000);
            m_AsyncAwait_flg = true;
        });
        */
    }


    // Invokeにてフラグを更新する
    void InvokeTest()
    {
        m_Invoke_flg = true;
    }


    // Start is called before the first frame update
    void Start()
    {
        // Invoke, Coroutine, async await の呼び出しを行う。
        Invoke("InvokeTest", m_interval);
        StartCoroutine(CoroutineTest());
        AsyncTest();

        // メッセージ表示を一定時間周期で行う処理をCoroutineにて実行。
        StartCoroutine(changeMessage());
    }


    // Update is called once per frame
    void Update()
    {
        m_deltaTime += Time.deltaTime;
        // deltaTime の時間蓄積値にて スフィアにAddForceを行う。
        if (m_deltaTime >= m_interval)
        {
            m_deltaTime = 0f;
            m_sphere_deltaTime.AddForce(m_vector);
        }

        // Invokeで変更されたフラグによりスフィアにAddForceを行う。
        if (m_Invoke_flg == true)
        {
            m_Invoke_flg = false;
            m_sphere_Invoke.AddForce(m_vector);
            Invoke("InvokeTest", m_interval);
        }

        // Coroutineで変更されたフラグによりスフィアにAddForceを行う。
        if (m_Coroutine_flg == true)
        {
            m_Coroutine_flg = false;
            m_sphere_Coroutine.AddForce(m_vector);
            StartCoroutine(CoroutineTest());
        }

        // async await + Task で変更されたフラグによりスフィアにAddForceを行う。
        if (m_AsyncAwait_flg == true)
        {
            m_AsyncAwait_flg = false;
            m_sphere_AsyncAwait.AddForce(m_vector);
            AsyncTest();
        }
    }
}
