﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    /******************************************************
    * Refer to the Photon documentation and scripting API for official definitions and descriptions
    * 
    * Documentation: https://doc.photonengine.com/en-us/pun/current/getting-started/pun-intro
    * Scripting API: https://doc-api.photonengine.com/en/pun/v2/index.html
    * 
    * If your Unity editor and standalone builds do not connect with each other but the multiple standalones
    * do then try manually setting the FixedRegion in the PhotonServerSettings during the development of your project.
    * https://doc.photonengine.com/en-us/realtime/current/connection-and-authentication/regions
    *
    * ******************************************************/
    [SerializeField]
    bool connectOnStart;

    [SerializeField]
    private int gameVersion;

    private void Start()
    {

        Debug.Log("start of  NetworkController  start()");

        if (connectOnStart)
        {
            Debug.Log("start() trying to connect");
            Connect();
        }
        else
        {
            Debug.Log("not trying because connectOnStart is False;");
            Connect();
        }

        Debug.Log("  end of  NetworkController  start()");
    }

    // Start is called before the first frame update
    public void Connect()
    {
        PhotonNetwork.GameVersion = gameVersion.ToString();
        PhotonNetwork.ConnectUsingSettings(); //Connects to Photon master servers
        //Other ways to make a connection can be found here: https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_pun_1_1_photon_network.html
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + " server!");
    }
}
