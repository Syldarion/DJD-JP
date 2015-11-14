using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrewClass;
using UpgradeClass;
using CargoClass;

namespace CrewClass
{
    public class Crew
    {
        int mCrew;
    }
}
namespace UpgradeClass
{
    public class Upgrade
    {

    }
}
namespace CargoClass
{
    public class Cargo
    {
        private int mFood;
        private int mGold;

        public int Food
        {
            get { return mFood; }
            set { mFood = value; }
        }
        public int Gold
        {
            get { return mGold; }
            set { mGold = value; }
        }

        public float PackFood(  )
        {
            return mFood / 100.0F;
        }

        public float PackGold(  )
        {
            return mGold / 1000.0F;
        }
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
        public List< Crew > mShipCrew
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
        public List< Upgrade > mShipUpgrades
        {
            get { return mShipUpgrades; }
            set { mShipUpgrades = value; }
        }
        public string mShipName
        {
            get { return mShipName; }
            set { mShipName = value; }
        }
        public Cargo mShipCargo;

        bool CheckCargoSpace(  )
        {
            bool canHoldMore = false;
            float totalCargo = mShipCargo.PackGold(  ) + mShipCargo.PackFood(  );

            if( totalCargo < mCargoSpace )  //  Is there any more cargo space available?
            {
            // If there is more space, return true.
                canHoldMore = true;
            }
            // If there isn't more space, do nothing and return false
            
            return canHoldMore;
        }
        bool AddCargo( Cargo toStore )
        {
            bool cargoWasStored = false;
            float amountOfCargoToStore = toStore.PackGold(  ) + toStore.PackFood(  );
            float amountofCargoStored = mShipCargo.PackGold(  ) + mShipCargo.PackFood(  );

            if( CheckCargoSpace(  ) )  // Can the ship hold more?
            {
            // If it can...
                if( ( amountofCargoStored + amountOfCargoToStore ) < mCargoSpace )  // Will the new cargo fit?
                {
                // If it does fit, put it in and the function returns true.
                    mShipCargo.Food += toStore.Food;
                    mShipCargo.Gold += toStore.Gold;
                    cargoWasStored = true;
                }
                // If it doesn't fit, don't do anything and the function returns false
            }
            // If it can't, don't do anything and the function returns false

            return cargoWasStored;
        }
        bool RemoveCargo( Cargo toRemove )
        {
            bool goldWasRemoved = false;
            bool foodWasRemoved = false;
            
            if( toRemove.Gold <= mShipCargo.Gold ) // Does the ship have the gold to remove?
            {
            // If it does, remove it.
                mShipCargo.Gold -= toRemove.Gold;
                goldWasRemoved = true;
            }
            // If it doesn't, do nothing.

            if( toRemove.Food < mShipCargo.Food ) // Does the ship have the food to remove?
            {
            // If it does, remove it.
                mShipCargo.Food -= toRemove.Food;
                foodWasRemoved = true;
            }
            // If it doesn't, do nothing.

            // If both removals succeeded, the function returns true.
            return goldWasRemoved && foodWasRemoved;
        }
        bool TradeCargo( Cargo toTrade, BaseShip from )
        {
            bool tradeSucceeded = false;
            float amountOfCargoToTrade = toTrade.PackGold(  ) + toTrade.PackFood(  );
            float amountOfCargoStored = mShipCargo.PackGold(  ) + mShipCargo.PackFood(  );

            if( this.CheckCargoSpace(  ) )  // Does the receiving ship have enough room?
            {
            // If it does...
                if( amountOfCargoToTrade + amountOfCargoStored < mCargoSpace )  // Will the incoming cargo fit?
                {
                // If it will...
                    if( toTrade.Gold < from.mShipCargo.Gold &&
                        toTrade.Food < from.mShipCargo.Food )  // Does the other ship have enough resources to make the trade?
                    {
                    // If it does, trade the resources and show the trade was sucessful.
                        this.AddCargo( toTrade );
                        from.RemoveCargo( toTrade );
                        tradeSucceeded = true;
                    }
                    // If it doesn't, do nothing and return false.
                }
                // If it won't, do nothing and return false.
            }
            // If it doesn't, do nothing and return false.

            return tradeSucceeded;
        }

        void DestroyShip(  )
        {
            mHullHealth = 0;
            mSailHealth = 0;
        }
    }
}
