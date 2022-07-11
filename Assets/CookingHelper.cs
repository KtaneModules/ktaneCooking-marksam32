using System;
using System.Linq;
using UnityEngine;

public class CookingHelper
{
    private static readonly int[,] MinutesMatrix = {
       { 10, 15, 20, 5, 30, 50 },
       { 75, 70, 80, 75, 65, 10},
       { 55, 70, 65, 50, 45, 60},
       { 95, 90, 75, 85, 70, 35},
       { 25, 30, 35, 20, 40, 10}
    };

    private readonly KMBombInfo kmBombInfo;
    private readonly int moduleId;

    public CookingHelper(int moduleId, KMBombInfo kmBombInfo)
    {
        this.moduleId = moduleId;
        this.kmBombInfo = kmBombInfo;
    }

    public void LogSolution()
    {
        var meal = this.GetMeal();
        var person = this.GetCookFor();
        Debug.LogFormat("[Cooking #{0}] Solution:", this.moduleId);
        Debug.LogFormat("[Cooking #{0}] - Cook for: {1}", this.moduleId, GetCookForName(person));
        Debug.LogFormat("[Cooking #{0}] - Meal: {1}", this.moduleId, GetMealName(meal));
        Debug.LogFormat("[Cooking #{0}] - Temp: {1} C", this.moduleId, MealToTemp(meal));
        Debug.LogFormat("[Cooking #{0}] - Time: {1} min", this.moduleId, GetCookingTime(meal, person));
        Debug.LogFormat("[Cooking #{0}] - Setting: {1}", this.moduleId, GetOvenSettingName(this.GetOvenSetting()));
        Debug.LogFormat("[Cooking #{0}] - Lamp: {1}", this.moduleId, this.ShouldLightBeTurnedOn);
    }

    private int GetCookingTime(Meal meal, Person person)
    {
        return MinutesMatrix[(int)meal - 1, (int)person - 1];
    }

    public bool Passed(int temp, int time, bool ovenLit, OvenSetting ovenSetting)
    {
        var meal = this.GetMeal();
        var person = this.GetCookFor();

        if (temp == MealToTemp(meal) && time == GetCookingTime(meal, person) && ovenLit == this.ShouldLightBeTurnedOn && ovenSetting == this.GetOvenSetting())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Meal GetMeal()
    {
        var mealNumber = this.kmBombInfo.GetBatteryHolderCount() - this.kmBombInfo.GetIndicators().Count() + 
            (this.kmBombInfo.GetBatteryCount() * this.kmBombInfo.GetPortCount()) - this.kmBombInfo.GetPortPlateCount();

        mealNumber = this.AdjustNumber(mealNumber, 5);

        return (Meal)mealNumber;
    }

    private static string GetMealName(Meal meal)
    {
        switch (meal)
        {
            case Meal.Pizza250C:
                return "Pizza";
            case Meal.SpaghettiBolognese160C:
                return "Spaghetti Bolognese";
            case Meal.ChickenCassrole200C:
                return "Chicken Casserole";
            case Meal.ChilliConCarne180C:
                return "Chilli ConCarne";
            case Meal.ChickenPie180C:
                return "Chicken Pie";
            default:
                throw new InvalidOperationException();
        }
    }

    public int MealToTemp(Meal meal)
    {
        switch (meal)
        {
            case Meal.Pizza250C:
                return 250;
            case Meal.SpaghettiBolognese160C:
                return 160;
            case Meal.ChickenCassrole200C:
                return 200;
            case Meal.ChilliConCarne180C:
                return 180;
            case Meal.ChickenPie180C:
                return 180;
            default:
                throw new InvalidOperationException();
        }
    }

    public OvenSetting GetOvenSetting()
    {
        var settingNumber = this.kmBombInfo.GetOnIndicators().Count() - this.kmBombInfo.GetOffIndicators().Count() + 
            this.kmBombInfo.GetSerialNumberLetters().Count();

        settingNumber = this.AdjustNumber(settingNumber, 6);

        return (OvenSetting)settingNumber;
    }

    public static string GetOvenSettingName(OvenSetting ovenSetting)
    {
        switch (ovenSetting)
        {
            case OvenSetting.BottomElementHeat:
                return "Bottom Element Heat";
            case OvenSetting.BottomElementHeatingwithGrill:
                return "Bottom Element Heat With Grill";
            case OvenSetting.ConventionalHeating:
                return "Conventional Heating";
            case OvenSetting.FanOven:
                return "Fan Oven";
            case OvenSetting.FanWithGrill:
                return "Fan With Grill";
            case OvenSetting.Grill:
                return "Grill";
            default:
                throw new InvalidOperationException();
        }
    }

    private Person GetCookFor()
    {
        if (this.CookForHarry)
        {
            return Person.Harry;
        }
        else if (this.CookForJames)
        {
            return Person.James;
        }
        else if (this.CookForTom)
        {
            return Person.Tom;
        }
        else if (this.CookforErik)
        {
            return Person.Erik;
        }
        else if (this.CookForBob)
        {
            return Person.Bob;
        }
        else
        {
            return Person.Markus;
        }
    }

    private static string GetCookForName(Person person)
    {
        switch (person)
        {
            case Person.Harry:
                return "Harry";
            case Person.James:
                return "James";
            case Person.Tom:
                return "Tom";
            case Person.Erik:
                return "Erik";
            case Person.Bob:
                return "Bob";
            case Person.Markus:
                return "Markus";
            default:
                throw new InvalidOperationException();
        }
    }

    public bool ShouldLightBeTurnedOn
    {
        get
        {
            return this.SerialNumberContainsVowels || this.HasPS2Port;
        }
    }

    private bool HasEmptyPortPlate
    {
        get
        {
            foreach (object[] plate in kmBombInfo.GetPortPlates())
            {
                if (plate.Length == 0)
                {
                    return true;

                }
            }
            return false;
        }
    }

    public int GetCookingTime()
    {
        return MinutesMatrix[(int)GetMeal() - 1, (int)GetCookFor() - 1];
    }

    private bool SerialNumberContainsVowels
    {
        get
        {
            return this.kmBombInfo.GetSerialNumber().Any(x => new[] { 'A', 'E', 'I', 'O', 'U', 'Y' }.Contains(x));
        }
    }

    private bool HasLitFRK
    {
        get
        {
            return this.kmBombInfo.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.FRK);
        }
    }

    private bool HasUnlitSND
    {
        get
        {
            return this.kmBombInfo.IsIndicatorOff(KMBombInfoExtensions.KnownIndicatorLabel.SND);
        }
    }

    private bool HasLitFRQ
    {
        get
        {
            return this.kmBombInfo.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.FRQ);
        }
    }

    private bool HasUnlitBOB
    {
        get
        {
            return this.kmBombInfo.IsIndicatorOff(KMBombInfoExtensions.KnownIndicatorLabel.BOB);
        }
    }

    private bool HasLitBOB
    {
        get
        {
            return this.kmBombInfo.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.BOB);
        }
    }

    private bool CookForHarry
    {
        get
        {
            return this.HasLitFRK || this.HasSerialPort;
        }
    }

    private bool CookForJames
    {
        get
        {
            return this.HasEmptyPortPlate || this.HasLitFRQ;
        }
    }

    private bool CookForTom
    {
        get
        {
            return this.MoreDigitsThanLettersInSerial || this.HasUnlitSND;
        }
    }

    private bool CookforErik
    {
        get
        {
            return this.HasCompositeVideoPort || this.HasHDMIPort || this.HasUSBPort;
        }
    }

    private bool CookForBob
    {
        get
        {
            return this.HasLitBOB || this.HasUnlitBOB;
        }
    }

    private bool HasSerialPort
    {
        get
        {
            return this.kmBombInfo.GetPortCount(KMBombInfoExtensions.KnownPortType.Serial) > 0;
        }
    }

    private bool MoreDigitsThanLettersInSerial
    {
        get
        {
            return this.kmBombInfo.GetSerialNumberNumbers().Count() > this.kmBombInfo.GetSerialNumberLetters().Count();
        }
    }

    private bool HasPS2Port
    {
        get
        {
            return this.kmBombInfo.GetPortCount(KMBombInfoExtensions.KnownPortType.PS2) > 0;
        }
    }

    private bool HasHDMIPort
    {
        get
        {
            return this.kmBombInfo.GetPortCount(KMBombInfoExtensions.KnownPortType.HDMI) > 0;
        }
    }

    private bool HasCompositeVideoPort
    {
        get
        {
            return this.kmBombInfo.GetPortCount(KMBombInfoExtensions.KnownPortType.CompositeVideo) > 0;
        }
    }

    private bool HasUSBPort
    {
        get
        {
            return this.kmBombInfo.GetPortCount(KMBombInfoExtensions.KnownPortType.USB) > 0;
        }
    }


    private int AdjustNumber(int value, int adjustWith)
    {
        if (value <= 0)
        {
            while (value <= 0)
            {
                value += adjustWith;
            }
        }
        else if (value > adjustWith)
        {
            while (value > adjustWith)
            {
                value -= adjustWith;
            }
        }

        return value;
    }

}

public enum Meal
{
    Pizza250C = 1,

    SpaghettiBolognese160C = 2,

    ChickenCassrole200C = 3,

    ChilliConCarne180C = 4,

    ChickenPie180C = 5
}

public enum OvenSetting
{
    BottomElementHeat = 1,

    BottomElementHeatingwithGrill = 2,

    ConventionalHeating = 3,

    FanOven = 4,

    Grill = 5,

    FanWithGrill = 6
}

public enum Person
{
    James = 1,

    Bob = 2,

    Markus = 3,

    Erik = 4,
    
    Harry = 5,

    Tom = 6

}

