using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public class Cooking : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombInfo Info;
    public KMBombModule Module;

    public TextMesh MinutesTxt;
    public TextMesh DegreeTxt;

    public KMSelectable[] DegreeBtns;
    public KMSelectable[] TimeBtns;
    public KMSelectable[] TypeBtns;
    public KMSelectable LightBtn;
    public KMSelectable CookBtn;

    public MeshRenderer Symbol;
    public Texture[] TypeSymbols;
    public Texture[] OvenDoorSates;
    public MeshRenderer OvenDoor;

    private static int _moduleIdCounter = 1;
    private int _moduleId;
    private bool isSolved = false;

    private int currentSymbol;
    private int ovenStartSymbol;
    private int temp;
    private int time;

    public CookingHelper cookingHelper;

    private void HandlePass()
    {
        var ovenSettings = (OvenSetting)(this.currentSymbol + 1);
        Debug.LogFormat("[Cooking #{0}] Submitted:", this._moduleId);
        Debug.LogFormat("[Cooking #{0}] - Temp {1} C",this._moduleId ,this.temp);
        Debug.LogFormat("[Cooking #{0}] - Time {1} min", this._moduleId, this.time);
        Debug.LogFormat("[Cooking #{0}] - Setting: {1}", this._moduleId, CookingHelper.GetOvenSettingName(ovenSettings));
        Debug.LogFormat("[Cooking #{0}] - Lamp {1}", this._moduleId, this.ovenStartSymbol == 1);

        if (this.cookingHelper.Passed(temp, time, ovenStartSymbol == 1, ovenSettings))
        {
            Debug.LogFormat("[Cooking #{0}] Correct! Module passed.",this._moduleId);
            Audio.PlaySoundAtTransform("ModuleSolved", this.Module.transform);
            Module.HandlePass(); 
            this.isSolved = true;
        }
        else
        {
            Debug.LogFormat("[Cooking #{0}] Wrong answer! Strike.", this._moduleId);
            Module.HandleStrike();
            this.isSolved = false;
        }
    }
    void Activate()
    {
        currentSymbol = UnityEngine.Random.Range(0, 6);
        Symbol.material.mainTexture = TypeSymbols[currentSymbol];
        ovenStartSymbol = 0;
        temp = 0;
        time = 0;

        cookingHelper = new CookingHelper(_moduleId, Info);
        cookingHelper.LogSolution();


        TypeBtns[0].OnInteract += delegate
        {
            TypeBtns[0].AddInteractionPunch();
            Audio.PlaySoundAtTransform("ButtonSound", this.TypeBtns[0].transform);
            if (this.isSolved == true)
            {
                return false;
            }
            currentSymbol = (currentSymbol - 1 < 0) ? 5 : currentSymbol - 1;
            Symbol.material.mainTexture = TypeSymbols[currentSymbol];
            return false;
        };
        TypeBtns[1].OnInteract += delegate
        {
            TypeBtns[1].AddInteractionPunch();
            Audio.PlaySoundAtTransform("ButtonSound", this.TypeBtns[1].transform);
            if (this.isSolved == true)
            {
                return false;
            }
            currentSymbol = (currentSymbol + 1 > 5) ? 0 : currentSymbol + 1;
            Symbol.material.mainTexture = TypeSymbols[currentSymbol];
            return false;
        };
        LightBtn.OnInteract += delegate
        {
            LightBtn.AddInteractionPunch();
            Audio.PlaySoundAtTransform("ButtonSound", this.LightBtn.transform);
            if (this.isSolved == true)
            {
                return false;
            }
            if (ovenStartSymbol == 0)
            {
                OvenDoor.material.mainTexture = OvenDoorSates[1];
                ovenStartSymbol = 1;
                return false;
            }
            else
            {
                OvenDoor.material.mainTexture = OvenDoorSates[0];
                ovenStartSymbol = 0;
                return false;
            }
        };
        DegreeBtns[0].OnInteract += delegate
        {
            DegreeBtns[0].AddInteractionPunch();
            Audio.PlaySoundAtTransform("ButtonSound", this.DegreeBtns[0].transform);
            if (this.isSolved == true)
            {
                return false;
            }
            if (temp > 0)
            {
                this.temp -= 10;
                this.DegreeTxt.text = temp.ToString();
            }
            return false;
        };
        DegreeBtns[1].OnInteract += delegate
        {
            DegreeBtns[1].AddInteractionPunch();
            Audio.PlaySoundAtTransform("ButtonSound", this.DegreeBtns[1].transform);
            if (this.isSolved == true)
            {
                return false;
            }
            if (temp < 250)
            {
                this.temp += 10;
                this.DegreeTxt.text = temp.ToString();
            }
            return false;
        };
        TimeBtns[0].OnInteract += delegate
        {
            TimeBtns[0].AddInteractionPunch();
            Audio.PlaySoundAtTransform("ButtonSound", this.TimeBtns[0].transform);
            if (this.isSolved == true)
            {
                return false;
            }
            if (time > 0)
            {
                this.time -= 5;
                this.MinutesTxt.text = time.ToString();
            }
            return false;
        };
        TimeBtns[1].OnInteract += delegate
        {
            TimeBtns[1].AddInteractionPunch();
            Audio.PlaySoundAtTransform("ButtonSound", this.TimeBtns[1].transform);
            if (this.isSolved == true)
            {
                return false;
            }
            if (time < 95)
            {
                this.time += 5;
                this.MinutesTxt.text = time.ToString();
            }
            return false;
        };
        CookBtn.OnInteract += delegate ()
        {
            CookBtn.AddInteractionPunch();
            Audio.PlaySoundAtTransform("ButtonSound", this.CookBtn.transform);
            if (this.isSolved == true)
            {
                return false;
            }
            this.HandlePass();
            return false;
        };

    }
    // Use this for initialization
    void Start ()
    {
        _moduleId = _moduleIdCounter++;
        Module.OnActivate += Activate;
       
    }
    //Twitch plays:
    KMSelectable[] ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();

        if (command.Equals ("toggle light"))
        {
            return new[] { LightBtn };
        }

        if (command.Equals("cook"))
        {
            return new[] { CookBtn };
        }

        var matchTemp = Regex.Match(command, @"^set temp (\d{1,3}$)");
        var matchTime = Regex.Match(command, @"^set time (\d{1,2}$)");
        var matchSetting = Regex.Match(command, @"^set setting (.+$)");
        if (matchTemp.Success)
        {
            var value = int.Parse(matchTemp.Groups[1].Value);
            return this.UpdateTemp(value);
        }

        if (matchTime.Success)
        {
            var value = int.Parse(matchTime.Groups[1].Value);
            return this.UpdateTime(value);
        }

        if (matchSetting.Success)
        {
            return this.UpdateSetting(matchSetting.Groups[1].Value);
        }

        return null;
    }

    private KMSelectable[] UpdateTemp(int temp)
    {
        if (temp % 10 == 0 && temp >= 0 && temp <= 250)
        {
            var selectables = new List<KMSelectable>();
            var diff = temp - this.temp;
            for (int i = 0; i < Math.Abs(diff); i = i + 10)
            {
                selectables.Add(diff > 0 ? DegreeBtns[1] : DegreeBtns[0]);
            }

	        if (selectables.Count == 0)
		        selectables.Add(null);

            return selectables.ToArray();
        }

        return null;
    }

    private KMSelectable[] UpdateTime(int time)
    {
        if (time % 5 == 0 && time >= 0 && time <= 95)
        {
            var selectables = new List<KMSelectable>();
            var diff = time - this.time;
            for (int i = 0; i < Math.Abs(diff); i = i + 5)
            {
                selectables.Add(diff > 0 ? TimeBtns[1] : TimeBtns[0]);
            }

	        if (selectables.Count == 0)
		        selectables.Add(null);

			return selectables.ToArray();
        }

        return null;
    }

    private KMSelectable[] UpdateSetting(string value)
    {
        if ("beh".Equals(value, StringComparison.InvariantCultureIgnoreCase) || "bottom element heat".Equals(value, StringComparison.InvariantCultureIgnoreCase))
        {
            return UpdateSetting(0);
        }

        if ("behwg".Equals(value, StringComparison.InvariantCultureIgnoreCase) || "bottom element heat with grill".Equals(value, StringComparison.InvariantCultureIgnoreCase))
        {
            return UpdateSetting(1);
        }

        if ("ch".Equals(value, StringComparison.InvariantCultureIgnoreCase) || "conventional heating".Equals(value, StringComparison.InvariantCultureIgnoreCase))
        {
            return UpdateSetting(2);
        }

        if ("fo".Equals(value, StringComparison.InvariantCultureIgnoreCase) || "fan oven".Equals(value, StringComparison.InvariantCultureIgnoreCase))
        {
            return UpdateSetting(3);
        }

        if ("g".Equals(value, StringComparison.InvariantCultureIgnoreCase) || "grill".Equals(value, StringComparison.InvariantCultureIgnoreCase))
        {
            return UpdateSetting(4);
        }

        if ("fwg".Equals(value, StringComparison.InvariantCultureIgnoreCase) || "fan with grill".Equals(value, StringComparison.InvariantCultureIgnoreCase))
        {
            return UpdateSetting(5);
        }


        return null;
    }

    public string TwitchHelpMessage = "Set the temperature using: !{0} set temp #. Set the time using: !{0} set time #. Set the setting using !{0} set setting bottom element heat. You can also use shortcuts: Bottom element heat = beh. grill = g etc. Toggle the light using: !{0} toggle light. Cook using: !{0} cook.";

    private KMSelectable[] UpdateSetting(int newSymbol)
    {
        var selectables = new List<KMSelectable>();
        var diff = newSymbol - this.currentSymbol;
        for (int i = 0; i < Math.Abs(diff); ++i)
        {
            selectables.Add(diff > 0 ? TypeBtns[1] : TypeBtns[0]);
        }

	    if (selectables.Count == 0)
		    selectables.Add(null);

		return selectables.ToArray();
    }
}
