using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;

public class MqttManager : MonoBehaviour
{
    private string clientId ="";

    public  string outTopic = "Moter4_01087700060/eachControl";
    public  string inTopic = "Moter4_01087700060/moterOutput";
    //public string outTopic = "t/eachControl";
    //public string inTopic = "t/moterOutput";

    private MqttClient client;

    public string recieveMessage;

    /// <summary>
    /// 서버로 부터 받은 버튼들의 상태.
    /// </summary>
    // public string PowerButtonState;
    public string Button_1_State;
    public string Button_2_State;
    public string Button_3_State;
    public string Button_4_State;
    public string Button_5_State;

    /// <summary>
    /// 버튼 최종 정렬하기 위해 한번 실행.
    /// </summary>
    public bool isOne; 

   // public GameObject buttonPowerObject;
    public GameObject button1Object;
    public GameObject button2Object;
    public GameObject button3Object;
    public GameObject button4Object;
    public GameObject button5Object;

    public GameObject loadingPopUpObject;
    public GameObject reConnectPopUpObject;
   // public bool isError; // error message 들어오면 팝업 띠워주자.
  //  public bool isReConnect; // 아두이노 wifi통신이 다시 접속했다는 메시지 창. -- 사용자가 불안해 한다.  고민하다 수정.
    public string currentButton;  // 명령 버튼명을 저장후  서버로 부터 받은 번호와 같은지 비교하기 위해 저장.
    public string currentButtonState;// 명령 버튼의 상태를 저장후  서버로 부터 받은 번호와 같은지 비교하기 위해 저장.
    public bool isLoading; // 버튼 누르고 로딩 화면 보여 줄려고.
    public float time; // 로딩이 3초 이상 될때는 화면을 꺼줘라. 다시 누를수 있도록.

    

    /// <summary>
    /// Plugins  -> Android 폴더를 생성하고  AndroidManifest.xml를 Android 폴더에 넣는다.
    /// C:\Program Files (x86)\Unity\Editor\Data\PlaybackEngines\androidplayer\AndroidManifest.xml  에
    /// 권한를 입력한다. <uses-permission android:name="android.permission.READ_PHONE_STATE"/>    <manifest></manifest> 안에 넣는다.
    /// 이것으로 안드로이드폰과의 연결이 완료 된다. 간단함.  (% 안드로이드의 jar 나 arr 만들 필요가 없음.)
    ///
    /// ------------------------>https://bspfp.pe.kr/archives/828   참조.
    /// WiFi MAC 주소를 가져오려면 ACCESS_WIFI_STATE 권한이 필요합니다.
    ///IMEI code 를 가져오려면 READ_PHONE_STATE 권한이 필요합니다.
    ///그럼 이 권한은 어떻게 설정할까요?
    ///만약 프로젝트 경로 아래에
    ///Assets/Plugins/Android/AndroidManifest.xml 파일이 있다면 해당 파일을 수정하시면 됩니다.
    ///이 파일이 없으시다면
    ///(Unity 설치 경로)/Editor/Data/PlaybackEngines 폴더 밑에 있는
    ///androidplayer 또는 androiddevelopmentplayer 폴더에 있는 파일을 복사해서 사용하시면 됩니다.
    /// -------------------------->
    /// </summary>
    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
            clientId = GetPhoneNumber();
    }

    void Start()
    {
        time = 0;
        // create client instance 
        client = new MqttClient(IPAddress.Parse("119.205.235.214"), 1883, false, null);

        // register to message received 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        if (clientId.Equals(string.Empty))
        {
            Debug.Log("client phone number empty");
            clientId = "test empty moter5 :" + Guid.NewGuid().ToString();
        }

        client.Connect(clientId);

        // subscribe to the topic "/home/temperature" with QoS 2 
        client.Subscribe(new string[] { inTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

       // SendPublishButtonData("ping", "ping");  시흥에는 없는 부분
        isOne = true;  
    }

    /// <summary>
    /// string 으로 객체나 함수를 불러와 적용시킨다.  멋진데.
    /// </summary>
    /// <returns></returns>
    public string GetPhoneNumber()
    {
        string imeiCode = "";
        using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
        {
            using (AndroidJavaObject telephonyManager = activity.Call<AndroidJavaObject>("getSystemService", activity.GetStatic<string>("TELEPHONY_SERVICE")))
            {
                // imeiCode = telephonyManager.Call<string>("getDeviceId");
                imeiCode = telephonyManager.Call<string>("getLine1Number");
                if (!string.IsNullOrEmpty(imeiCode))
                {
                    return imeiCode.Replace("+82", "0");
                }
            }
        }
        return "error";
    }

    /// <summary>
    /// 버튼 클릭 현재 정보를 전달한다.
    /// </summary>
    /// <param name="topic">버튼 주소</param>
    public void SendPublishButtonData(string sendData)
    {
        client.Publish( outTopic, System.Text.Encoding.Default.GetBytes(sendData));     
    }
 
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {    
        // 검증 하고 (보낸 번호와 버튼이 같은지 ) 아니라면 3번 전송.
        if (e.Topic == inTopic )
        {
            recieveMessage = System.Text.Encoding.UTF8.GetString(e.Message);

            Debug.Log("M : " + recieveMessage);

           AllMessageParsing(recieveMessage);
            //각 버튼들 정렬 - 현재 받은 값으로 
           isOne = true;
        }
        else
        {
            Debug.Log("맞지 않는 토픽 : " + e.Topic);
        }
    }

    /// <summary>
    /// 단순한 메시지 변환 1->true   0->false
    /// </summary>
    /// <param name="message">변환할 문자.</param>
    /// <returns></returns>
    public bool GetBoolMessageChange(string message)
    {        
        bool v = false;
        if (message == "pinOn")  
            v = true;
        else if (message == "pinOff")     
            v = false;
        else if (String.IsNullOrEmpty(message))
        {
            v = false;
           // isError = true;
            Debug.Log("empty message:" + v);
        }        
        else if(message == "Reconnected")
        {
            v = false;
           // isError = true;
            Debug.Log("Recon : " + message + ":" + v);
        }
        else
        {
            v = false;
            // isError = true;
            Debug.Log("잘못된 명령 메시지 입니다. : " + message + ":" + v);
        }
        return v;
    }

    void Update()
    {
        if (isOne)
        {
            StartCoroutine(AllButtonSet());            
            isOne = false;          
        }

        //팝업 메시지 띠우기.
       loadingPopUpObject.SetActive(isLoading);
    }

    /// <summary>
    /// 약간의 딜레이를 주기 위해.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AllButtonSet()
    {        
        yield return new WaitForSeconds(.1f);
        AllButtonsSetting();
    }

    /// <summary>
    /// 서버로 부터 받은 정보를 각각의 버튼들에게 전달한다.
    /// </summary>
    public void AllButtonsSetting()
    {   
        button1Object.GetComponent<SwitchingManager>().SetSwitching(GetBoolMessageChange(Button_1_State));
        button2Object.GetComponent<SwitchingManager>().SetSwitching(GetBoolMessageChange(Button_2_State));
        button3Object.GetComponent<SwitchingManager>().SetSwitching(GetBoolMessageChange(Button_3_State));
        button4Object.GetComponent<SwitchingManager>().SetSwitching(GetBoolMessageChange(Button_4_State));
        button5Object.GetComponent<SwitchingManager>().SetSwitching(GetBoolMessageChange(Button_5_State));
    }
       
    /// <summary>
    /// 서버로 부터 받은 정보를 각 변수에 저장한다.
    /// </summary>
    /// <param name="getMessage">서버로 부터 받은 정보.</param>
    private void AllMessageParsing(string getMessage)
    {
        PacketAll packetAll = JsonUtility.FromJson<PacketAll>(getMessage);
        Button_1_State = string.IsNullOrEmpty(packetAll.buttonState1) ? "pinOff" : packetAll.buttonState1;
        Button_2_State = string.IsNullOrEmpty(packetAll.buttonState2) ? "pinOff" : packetAll.buttonState2;
        Button_3_State = string.IsNullOrEmpty(packetAll.buttonState3) ? "pinOff" : packetAll.buttonState3;
        Button_4_State = string.IsNullOrEmpty(packetAll.buttonState4) ? "pinOff" : packetAll.buttonState4;
        Button_5_State = string.IsNullOrEmpty(packetAll.buttonState5) ? "pinOff" : packetAll.buttonState5;
    }

    /// <summary>
    /// 보낸 버튼의 신호를 확인하고 다른 값이 라면  보낸 값될때 까지 3초간 지속적으로 보낸다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ReSendToServer(string orderMessage)
    {
        SendPublishButtonData(orderMessage);

        yield return new WaitForSeconds(.1f);
        if (currentButton == "1")
        {
            if (currentButtonState != Button_1_State)
            {
                //Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(orderMessage);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    StopCoroutine("ReSendToServer");
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }
                //Debug.Log("Message ReSend To Server 2");
              StartCoroutine(ReSendToServer(orderMessage));
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        else if (currentButton == "2")
        {
            if (currentButtonState != Button_2_State)
            {
               // Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(orderMessage);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    StopCoroutine("ReSendToServer");
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }
                //Debug.Log("Message ReSend To Server 2");
                    StartCoroutine(ReSendToServer(orderMessage));
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        else if (currentButton == "3")
        {
            if (currentButtonState != Button_3_State)
            {
               // Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(orderMessage);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    StopCoroutine("ReSendToServer");
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }
               // Debug.Log("Message ReSend To Server 2");
                
               StartCoroutine(ReSendToServer(orderMessage));
               
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        else if (currentButton == "4")
        {
            if (currentButtonState != Button_4_State)
            {
                //Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(orderMessage);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    StopCoroutine("ReSendToServer");
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }

                //Debug.Log("Message ReSend To Server 2");
                
                 StartCoroutine(ReSendToServer(orderMessage));
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        else if (currentButton == "5")
        {
            if (currentButtonState != Button_5_State)
            {
                //Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(orderMessage);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    StopCoroutine("ReSendToServer");
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }

                //Debug.Log("Message ReSend To Server 2");

                StartCoroutine(ReSendToServer(orderMessage));
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        /*
        else if (currentButton == "buttonPower")
        {
            if (currentButtonState != PowerButtonState)
            {
                //Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(currentButton, currentButtonState);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }
               // Debug.Log("Message ReSend To Server 2");
                
               StartCoroutine(ReSendToServer());
               
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        */
        else
        {
            Debug.Log("아런 경우도 있나? 버튼이");
        }
    }

 /*

    /// <summary>
    /// 서버로 부터 받은 정보를 나눈다.
    /// </summary>
    /// <param name="message">서버 data</param>
    /// <param name="startSearch">시작문구</param>
    /// <param name="endSearch">끝 문구</param>
    /// <returns></returns>
    public string GetParserString(string message ,string startSearch,string endSearch)
    {
        string getValue = "";
        string search = "";

        search = startSearch;        
    
        int p = message.IndexOf(search);
        if (p >= 0)
        {
            // move forward to the value
            int start = p + search.Length;
            // now find the end by searching for the next closing tag starting at the start position, 
            // limiting the forward search to the max value length
            int end = 0;
            end = message.IndexOf(endSearch, start);           

            if (end >= 0)
            {
                // pull out the substring
                string v = message.Substring(start, end - start);
                // finally parse into a float
                // float value = float.Parse(v);
                // Debug.Log("1classTemp Value = " + value);
              
               getValue = v;                
            }
            else
            {
                Debug.Log("Bad html - closing tag not found");
                getValue = "text error";
            }
        }
        return getValue;
    }
    */
    /// <summary>
    /// 버튼의 스위칭 명령   0->1   1->0변환 명령.
    /// 이유: 반대로 보여 줘야 한다.
    /// 끔(0)을 아두이노로 보낸다.(끔버튼을 누를때)  -> 스위칭된 현재 버튼은 반대의 켬상태로 보여진다.(켜진줄 안다)
    /// 켬(1)을 아두이노로 보낸다.(켬버튼을 누를때) -> 스위칭된 현재 버튼은  반대의 꺼짐상태가 된다.(꺼진줄 안다)
    /// 
    /// 아두이노로 부터 0을 다시 받기위해선 0을 보내야 한다.
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    public string SendOrder(string order)
    {
        string v = "";
        if (order == "pinOn")
            v = "pinOff";
        else if (order == "pinOff")
            v = "pinOn";
       // else if (string.IsNullOrEmpty(order))
       //     v = "pinOff";
        else
            Debug.Log("SendOrder 함수 오류");
        return v;
    }

}
