using CombatLogOptions.Utility;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.CombatLog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CombatLogOptions.CombatLog
{
    [Serializable]
    internal class Builder
    {
        private bool _isLoaded = false;

        [JsonProperty("Options")]
        public Dictionary<string, float> CurrentOptions { get; private set; }
        public List<TextMeshProUGUI> TextMeshes;

        public void Load()
        {
            if (_isLoaded) return;

            CurrentOptions = [];
            TextMeshes = [];

            try
            {
                var path = Path.Combine(Main.ModPath, Options.OPTIONS_FILE);

                if (!File.Exists(path))
                {
                    Main.Logger.Warning($"{Options.OPTIONS_FILE} not found. This is nomal only when mod is run for the first time.");
                    return;
                }

                CurrentOptions = JSON.FromJSON<Dictionary<string, float>>(File.ReadAllText(path));

                if (CurrentOptions == null)
                {
                    Main.Logger.Error($"Error loading {Options.OPTIONS_FILE}");
                    throw new FileNotFoundException();
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex);
            }
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(Path.Combine(Main.ModPath, Options.OPTIONS_FILE), JSON.ToJSON(CurrentOptions));
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex);
            }
        }

        public void Build()
        {
            try
            {
                Load();

                TextMeshes.Clear();

                var combatLog = Game.Instance.UI.Canvas.transform.root.Find("InGameStaticPartPCView/LogCanvas/HUDLayout/CombatLog_New")
                    ?? throw new NullReferenceException("combat log");
                var panel = combatLog.Find("Panel")
                    ?? throw new NullReferenceException("panel");
                var panelScrollView = panel.Find("Scroll View")
                    ?? throw new NullReferenceException("Panel Scroll View");
                var tooglePanel = combatLog.Find("TooglePanel")
                    ?? throw new NullReferenceException("toogle panel");
                var toogleButtonTransform = tooglePanel.Find("ToogleLifeEvent")
                    ?? throw new NullReferenceException("toogle button");
                var tmpTemplate = combatLog.GetComponent<CombatLogPCView>()?.m_LogItemView?.m_Text
                    ?? throw new NullReferenceException("tmpTemplate");
                var backGroundImage = panel.Find("Background/Background_Image")?.GetComponent<Image>()
                    ?? throw new NullReferenceException("background image");
                var backGroundShadowMaskImage = panel.Find("Background/Background_Shadow_Mask/Background_Shadow")?.GetComponent<Image>()
                    ?? throw new NullReferenceException("background shadow mask image");

                var panelCavas = panel.gameObject.AddComponent<CanvasGroup>();

                var comScrollRect = BuildScrollRect(Prefabs.CombatLogOptions, panel);

                BuildHeader(Prefabs.HeaderTemplate, comScrollRect.content, "Panel Canvas", tmpTemplate);
                panelCavas.alpha = BuildSliderGroup(
                    Prefabs.SliderGroupTemplate,
                    comScrollRect.content,
                    "Alpha",
                    Options.CANVAS_ALPHA,
                    tmpTemplate,
                    panelCavas.alpha,
                    (x) =>
                    {
                        if (x > .25f)
                            panelCavas.alpha = x;
                    });

                BuildHeader(Prefabs.HeaderTemplate, comScrollRect.content, "Background Image", tmpTemplate);
                backGroundImage.color = BuildSliderGroupsImage(
                    Prefabs.SliderGroupTemplate,
                    comScrollRect.content,
                    Options.BACKGROUND_GROUP,
                    tmpTemplate,
                    backGroundImage);

                BuildHeader(Prefabs.HeaderTemplate, comScrollRect.content, "Background Shadow Mask", tmpTemplate);
                backGroundShadowMaskImage.color = BuildSliderGroupsImage(
                    Prefabs.SliderGroupTemplate,
                    comScrollRect.content,
                    Options.SHADOW_GROUP,
                    tmpTemplate,
                    backGroundShadowMaskImage);

                BuildHeader(Prefabs.HeaderTemplate, comScrollRect.content, "Text Highlighting", tmpTemplate);
                BuildSliderGroupsTextHighlight(
                    Prefabs.SliderGroupTemplate,
                    comScrollRect.content,
                    Options.HIGHLIGHT_GROUP,
                    tmpTemplate);
                BuildSliderGroup(
                    Prefabs.SliderGroupTemplate,
                    comScrollRect.content,
                    "Dilate",
                    Options.HIGHLIGHT_DILATE,
                    tmpTemplate,
                    0f,
                    (x) => UpdateTextmeshHighlights(),
                    minValue: -1f);
                BuildSliderGroup(
                    Prefabs.SliderGroupTemplate,
                    comScrollRect.content,
                    "Softness",
                    Options.HIGHLIGHT_SOFTNESS,
                    tmpTemplate,
                    0f,
                    (x) => UpdateTextmeshHighlights());

                BuildToogleButton(toogleButtonTransform, tooglePanel, comScrollRect.transform, panelScrollView);

                UpdateTextmeshHighlights();
            }
            catch (Exception ex)
            {
                var message = $"Unable to find {ex.Message}. {ex.StackTrace}";
                Main.Logger.Error(message);
            }
        }

        public ScrollRect BuildScrollRect(GameObject prefab, Transform parent)
        {
            var scroll = GameObject.Instantiate(prefab, parent);
            scroll.transform.localPosition = new(3f, 12.78f, 0f);
            (scroll.transform as RectTransform).sizeDelta = new(-10, -60);
            var scrollRect = scroll.GetComponent<ScrollRect>();
            scrollRect.scrollSensitivity = 10f;
            scroll.SetActive(false);

            return scrollRect;
        }

        public void CopyTMPFontAndMaterials(TextMeshProUGUI dest, TextMeshProUGUI tmpTemplate)
        {
            dest.font = tmpTemplate.font;
            dest.fontMaterial = tmpTemplate.material;
            dest.richText = tmpTemplate.richText;
        }

        public Toggle BuildToggleOption(GameObject prefab, Transform parent, string onText, string offText, TextMeshProUGUI tmpTemplate)
        {
            var toggleObj = GameObject.Instantiate(prefab, parent);
            var toggle = toggleObj.GetComponentInChildren<Toggle>();
            var toggleTMP = toggle.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            var onImg = toggle.transform.Find("OnImage");
            var offImg = toggle.transform.Find("OffImage");

            CopyTMPFontAndMaterials(toggleTMP, tmpTemplate);

            toggle.onValueChanged.AddListener((x) =>
            {
                toggleTMP.text = x ? onText : offText;
                onImg.gameObject.SetActive(x);
                offImg.gameObject.SetActive(!x);
            });

            toggleTMP.text = offText;
            toggle.isOn = false;
            toggleObj.gameObject.SetActive(true);

            return toggle;
        }

        public void BuildHeader(GameObject prefab, Transform parent, string text, TextMeshProUGUI tmpTemplate)
        {
            var header = GameObject.Instantiate(prefab, parent);
            var headerTMP = header.GetComponentInChildren<TextMeshProUGUI>();
            headerTMP.fontSizeMax = 30;
            headerTMP.alignment = TextAlignmentOptions.MidlineLeft;

            headerTMP.text = $"<voffset=0em><color=#672B31><size=120%>{text[0]}</size></color></voffset>{text.Substring(1)}";
            CopyTMPFontAndMaterials(headerTMP, tmpTemplate);
            TextMeshes.Add(headerTMP);

            header.SetActive(true);
        }

        public void UpdateTextmeshHighlights()
        {
            foreach (var mesh in TextMeshes)
            {
                mesh.SetUnderLay(
                        new(
                            CurrentOptions[Options.HIGHLIGHT_RED],
                            CurrentOptions[Options.HIGHLIGHT_GREEN],
                            CurrentOptions[Options.HIGHLIGHT_BLUE],
                            CurrentOptions[Options.HIGHLIGHT_ALPHA]),
                        CurrentOptions[Options.HIGHLIGHT_DILATE],
                        CurrentOptions[Options.HIGHLIGHT_SOFTNESS]);
            }
        }

        public Color BuildSliderGroupsTextHighlight(GameObject prefab, Transform parent, string optionGroupKey, TextMeshProUGUI tmpTemplate)
        {
            try
            {
                var r = BuildSliderGroup(prefab, parent, "Red", Options.OptionGroups[optionGroupKey]["RED"], tmpTemplate, 0f, (x) => UpdateTextmeshHighlights());
                var g = BuildSliderGroup(prefab, parent, "Green", Options.OptionGroups[optionGroupKey]["GREEN"], tmpTemplate, 0f, (x) => UpdateTextmeshHighlights());
                var b = BuildSliderGroup(prefab, parent, "Blue", Options.OptionGroups[optionGroupKey]["BLUE"], tmpTemplate, 0f, (x) => UpdateTextmeshHighlights());
                var a = BuildSliderGroup(prefab, parent, "Alpha", Options.OptionGroups[optionGroupKey]["ALPHA"], tmpTemplate, 0f, (x) => UpdateTextmeshHighlights());

                return new Color(r, g, b, a);
            }
            catch (Exception e)
            {
                Main.Logger.Error(e);
                return default;
            }
        }

        public Color BuildSliderGroupsImage(GameObject prefab, Transform parent, string optionGroupKey, TextMeshProUGUI tmpTemplate, Image image)
        {
            try
            {

                var r = BuildSliderGroup(prefab, parent, "Red", Options.OptionGroups[optionGroupKey]["RED"], tmpTemplate, image.color.r,
                        (x) =>
                        {
                            image.color = new Color(
                                x,
                                image.color.g,
                                image.color.b,
                                image.color.a);
                        });
                var g = BuildSliderGroup(prefab, parent, "Green", Options.OptionGroups[optionGroupKey]["GREEN"], tmpTemplate, image.color.g,
                        (x) =>
                        {
                            image.color = new Color(
                                image.color.r,
                                x,
                                image.color.b,
                                image.color.a);
                        });
                var b = BuildSliderGroup(prefab, parent, "Blue", Options.OptionGroups[optionGroupKey]["BLUE"], tmpTemplate, image.color.b,
                        (x) =>
                        {
                            image.color = new Color(
                                image.color.r,
                                image.color.g,
                                x,
                                image.color.a);
                        });
                var a = BuildSliderGroup(prefab, parent, "Alpha", Options.OptionGroups[optionGroupKey]["ALPHA"], tmpTemplate, image.color.a,
                        (x) =>
                        {
                            image.color = new Color(
                                image.color.r,
                                image.color.g,
                                image.color.b,
                                x);
                        });

                return new Color(r, g, b, a);
            }
            catch (Exception e)
            {
                Main.Logger.Error(e);
                return default;
            }
        }

        public float BuildSliderGroup(GameObject prefab, Transform parent, string text, string key, TextMeshProUGUI tmpTemplate, float initalValue, Action<float> OnChanged = null, float minValue = 0, float maxValue = 1)
        {
            var slider = GameObject.Instantiate(prefab, parent);
            var labelTMP = slider.transform.Find("Label").gameObject.GetComponent<TextMeshProUGUI>();
            var valueTMP = slider.transform.Find("Slider").gameObject.GetComponentInChildren<TextMeshProUGUI>();
            var sliderComp = slider.GetComponentInChildren<Slider>();

            labelTMP.text = text;

            CopyTMPFontAndMaterials(labelTMP, tmpTemplate);
            CopyTMPFontAndMaterials(valueTMP, tmpTemplate);
            TextMeshes.Add(labelTMP);

            sliderComp.minValue = minValue;
            sliderComp.maxValue = maxValue;
            sliderComp.onValueChanged.AddListener((x) =>
            {
                valueTMP.text = $"{x:0.00}";
                if (CurrentOptions.ContainsKey(key))
                    CurrentOptions[key] = x;
            });

            if (CurrentOptions.TryGetValue(key, out var value))
                initalValue = value;
            else
                CurrentOptions.Add(key, initalValue);

            sliderComp.value = initalValue;
            valueTMP.text = $"{initalValue:0.00}";

            if (OnChanged != null)
                sliderComp.onValueChanged.AddListener((x) => OnChanged(x));

            slider.SetActive(true);

            return initalValue;
        }

        public void BuildToogleButton(Transform buttonTransformTemplate, Transform parent, Transform optionsSV, Transform vanillaSV)
        {
            var button = GameObject.Instantiate(buttonTransformTemplate, parent);
            var toggle = button.GetComponent<Toggle>();

            button.name = "ToogleOptions";
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Options";

            toggle.onValueChanged.AddListener((x) =>
            {
                optionsSV.gameObject.SetActive(x);
                vanillaSV.gameObject.SetActive(!x);

                if (!x)
                    Save();
                else
                    UpdateTextmeshHighlights();
            });
        }
    }
}
