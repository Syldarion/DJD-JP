using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrewClass;
using UpgradeClass;

namespace CrewClass
{
    public class Crew
    {
        public int mMorale
        {
            get { return mMorale; }
            set { mMorale = value; }
        }

        public int mHealth
        {
            get { return mHealth; }
            set { mHealth = value; }
        }
    }
}

namespace UpgradeClass
{
    public class Upgrade
    {

    }
}

namespace ShipClass
{
    public class BaseShip
    {
        public int mHullHealth
        {
            get { return mHullHealth; }
            set { mHullHealth = value; }
        }

        public int mSailHealth
        {
            get { return mSailHealth; }
            set { mSailHealth = value; }
        }

        public List<Crew> mShipCrew
        {
            get { return mShipCrew; }
            set { mShipCrew = value; }
        }

        public double mCrewMorale
        {
            get { return mCrewMorale; }
            set { mCrewMorale = value; }
        }

        public int mCargoSpace
        {
            get { return mCargoSpace; }
            set { mCargoSpace = value; }
        }

        public List<Upgrade> mShipUpgrades
        {
            get { return mShipUpgrades; }
            set { mShipUpgrades = value; }
        }

        private string mShipName
        {
            get { return mShipName; }
            set { mShipName = value; }
        }
    }
}
