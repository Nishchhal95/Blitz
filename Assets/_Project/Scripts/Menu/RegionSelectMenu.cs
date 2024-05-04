using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegionSelectMenu : Menu
{
    [SerializeField] private Transform regionListHolder;
    [SerializeField] private RegionMenuItem regionMenuItemPrefab;
    private Dictionary<string, RegionMenuItem> regionCodeToMenuItem = new Dictionary<string, RegionMenuItem>();

    protected override void OnEnable()
    {
        base.OnEnable();
        Init();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void Init()
    {
        foreach (RegionMenuItem regionMenuItem in regionCodeToMenuItem.Values.ToList())
        {
            Destroy(regionMenuItem.gameObject);
        }
        regionCodeToMenuItem.Clear();

        Dictionary<string, string> regionMap = PhotonNetworkManager.GetRegionsMap();

        foreach (KeyValuePair<string, string> region in regionMap)
        {
            RegionMenuItem regionMenuItem = Instantiate(regionMenuItemPrefab, regionListHolder);
            regionMenuItem.RegionSelect += SetRegion;
            regionCodeToMenuItem.Add(region.Key, regionMenuItem);
            regionMenuItem.SetData(region.Key, region.Value);
        }
    }

    private void SetRegion(string regionCode)
    {
        PhotonNetworkManager.ChangeRegion(regionCode);
    }
}
