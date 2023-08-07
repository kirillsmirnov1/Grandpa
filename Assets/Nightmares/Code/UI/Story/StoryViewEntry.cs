﻿using Nightmares.Code.Model;
using TMPro;
using UnityEngine;
using UnityUtils.View;

namespace Nightmares.Code.UI.Story
{
    public class StoryViewEntry : ListViewEntry<StoryEntryData>
    {
        [SerializeField] private TextMeshProUGUI header;
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private GameObject blurObject;
        
        public override void Fill(StoryEntryData data)
        {
            header.text = data.title;
            mainText.text = data.mainText;
            blurObject.SetActive(!data.unlocked);
            base.Fill(data);
        }
    }
}