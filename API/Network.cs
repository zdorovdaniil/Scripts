using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;

public static class Network
{
    public static CloudRegionCode ConvertToRegionCode(int code)
    {
        if (code == 0) { return CloudRegionCode.eu; }
        else if (code == 1) { return CloudRegionCode.ru; }
        else if (code == 2) { return CloudRegionCode.rue; }
        else { return CloudRegionCode.eu; }
    }
    public static IEnumerator CheckInternetConnection(Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            Debug.Log("Error");
            action(false);
        }
        else
        {
            Debug.Log("Success");
            action(true);
        }
    }
}
