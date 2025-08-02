using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIStateHandler : MonoBehaviour
{
    // SINGLETON CLASS FOR MANAGING STATE
    public static UIStateHandler UISingleton;
    public enum Page
    {
        start,
        query,
        map
    }

    public GameObject StartView;
    public GameObject QueryView;
    public GameObject MapView;

    public List<GameObject> pageStack;


    public static Page ACTIVEPAGE = Page.start;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UISingleton = this;
        pageStack = new List<GameObject> { StartView, QueryView, MapView };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetQueryView()
    {
        ACTIVEPAGE = Page.query;
        foreach (GameObject page in pageStack.Where((x) => { return x != QueryView;}))
        {
            page.SetActive(false);
        }
        QueryView.SetActive(true);

    }

    public void SetMapView()
    {
        ACTIVEPAGE = Page.map;
        foreach (GameObject page in pageStack.Where((x) => { return x != MapView; }))
        {
            page.SetActive(false);
        }
        MapView.SetActive(true);
    }

    public void SetStartView()
    {
        ACTIVEPAGE = Page.start;
        foreach (GameObject page in pageStack.Where((x) => { return x != MapView; }))
        {
            page.SetActive(false);
        }
        StartView.SetActive(true);
    }




}
