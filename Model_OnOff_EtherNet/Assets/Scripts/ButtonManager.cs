using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public bool pingPong;
    private MqttManager mqttManager;

    void Start()
    {
        mqttManager = GameObject.Find("UI Root (3D)").GetComponent<MqttManager>();
        pingPong = false;
        //SendAllButtonSetting();
    }
    private void OnClick()
    {
        PacketEche packetEche = new PacketEche();

       // if (transform.name == "reConnectButton")
      //      mqttManager.isReConnect = false;
        if (transform.name == "loadingButton")
            mqttManager.isLoading = false;
        if (transform.name == "Button - Exit")
            Application.Quit();
        if (transform.name == "Button_Moter_1")
        {
            mqttManager.currentButton = "1";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_1_State);  // 보내려하는 버튼은 항상 현재 상태와 반대한다.(반대하는 명령)

            if (mqttManager.Button_1_State != mqttManager.currentButtonState) // 버튼의 반대 명령이 확인 됬다면.
            {
                packetEche.buttonNum = mqttManager.currentButton;
                packetEche.buttonState = mqttManager.currentButtonState;

                string msg = JsonUtility.ToJson(packetEche);
                mqttManager.SendPublishButtonData(msg);

                //정상.  버튼의 상태를 보여주면 된다.  아니라면  다시 보낸다.

                StartCoroutine(mqttManager.ReSendToServer(msg));
            }
        }
        if (transform.name == "Button_Moter_2")
        {
            mqttManager.currentButton = "2";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_2_State);
            if (mqttManager.Button_2_State != mqttManager.currentButtonState) // 버튼의 반대 명령이 확인 됬다면.
            {
                packetEche.buttonNum = mqttManager.currentButton;
                packetEche.buttonState = mqttManager.currentButtonState;

                string msg = JsonUtility.ToJson(packetEche);
                mqttManager.SendPublishButtonData(msg);

                //정상.  버튼의 상태를 보여주면 된다.  아니라면  다시 보낸다.

                StartCoroutine(mqttManager.ReSendToServer(msg));
            }
        }
        if (transform.name == "Button_Moter_3")
        {
            mqttManager.currentButton = "3";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_3_State);
            if (mqttManager.Button_3_State != mqttManager.currentButtonState) // 버튼의 반대 명령이 확인 됬다면.
            {
                packetEche.buttonNum = mqttManager.currentButton;
                packetEche.buttonState = mqttManager.currentButtonState;

                string msg = JsonUtility.ToJson(packetEche);
                mqttManager.SendPublishButtonData(msg);

                //정상.  버튼의 상태를 보여주면 된다.  아니라면  다시 보낸다.

                StartCoroutine(mqttManager.ReSendToServer(msg));
            }
        }
        if (transform.name == "Button_Moter_4")
        {
            mqttManager.currentButton = "4";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_4_State);
            if (mqttManager.Button_4_State != mqttManager.currentButtonState) // 버튼의 반대 명령이 확인 됬다면.
            {
                packetEche.buttonNum = mqttManager.currentButton;
                packetEche.buttonState = mqttManager.currentButtonState;

                string msg = JsonUtility.ToJson(packetEche);
                mqttManager.SendPublishButtonData(msg);

                //정상.  버튼의 상태를 보여주면 된다.  아니라면  다시 보낸다.

                StartCoroutine(mqttManager.ReSendToServer(msg));
            }
        }
        if (transform.name == "Button_Moter_5")
        {
            mqttManager.currentButton = "5";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_5_State);
            if (mqttManager.Button_5_State != mqttManager.currentButtonState) // 버튼의 반대 명령이 확인 됬다면.
            {
                packetEche.buttonNum = mqttManager.currentButton;
                packetEche.buttonState = mqttManager.currentButtonState;

                string msg = JsonUtility.ToJson(packetEche);
                mqttManager.SendPublishButtonData(msg);

                //정상.  버튼의 상태를 보여주면 된다.  아니라면  다시 보낸다.

                StartCoroutine(mqttManager.ReSendToServer(msg));
            }
        }
        /*
        if (transform.name == "Button_Power")
        {
            mqttManager.currentButton = "buttonPower";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.PowerButtonState);
            mqttManager.SendPublishButtonData(mqttManager.currentButton, mqttManager.currentButtonState);
            StartCoroutine(mqttManager.ReSendToServer());
        }    
        */
    }
}
