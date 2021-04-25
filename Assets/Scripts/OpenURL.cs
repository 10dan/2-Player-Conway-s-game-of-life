using UnityEngine;

public class OpenURL : MonoBehaviour {
    //Simple code that opens url in browser.
    public string Url;
    public void Open() {
        Application.OpenURL(Url);
    }
}
