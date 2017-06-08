using Symbioz.DofusProtocol.Messages;
using Symbioz.World.Models.Fights.Damages;
using Symbioz.World.Models.Fights.Fighters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.World.Models.Fights.Damages
{
    public class TakenDamages
    {
        public short Delta { get; set; }

        private bool Evaluated = false;

        public ElementType ElementType { get; set; }

        public TakenDamages(short delta, ElementType elementtype)
        {
            this.Delta = delta;
            this.ElementType = elementtype;
        }
        public short GetDeltaPercentage(sbyte percentage)
        {
           return (short)((double)Delta * ((double)percentage / 100.0));
        }
        /// <summary>
        ///  Dégâts subis =dégâts totaux - résistance fixe - (dégâts totaux / 100 / pourcentage de résistance) 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public void EvaluateWithResistances(Fighter source,Fighter target,bool pvp)
        {
            if (source == null || Evaluated)
                return;
            Evaluated = true;
            short elementBonus = 0;
            short elementReduction = 0;
            double elementResistPercent = 0;
            switch (ElementType)
            {
                case ElementType.Earth:
                    elementBonus = source.FighterStats.Stats.EarthDamageBonus;
                    elementReduction = (short)(target.FighterStats.Stats.EarthReduction + (pvp ? target.FighterStats.Stats.PvPEarthReduction : 0));
                    elementResistPercent = target.FighterStats.Stats.EarthResistPercent + (pvp ? target.FighterStats.Stats.PvPEarthResistPercent : 0);
                    break;
                case ElementType.Air:
                    elementBonus = source.FighterStats.Stats.AirDamageBonus;
                    elementReduction = (short)(target.FighterStats.Stats.AirReduction + (pvp ? target.FighterStats.Stats.PvPAirReduction : 0));
                    elementResistPercent = target.FighterStats.Stats.AirResistPercent + (pvp? target.FighterStats.Stats.PvPAirResistPercent : 0);
                    break;
                case ElementType.Water:
                    elementBonus = source.FighterStats.Stats.WaterDamageBonus;
                    elementReduction = (short)(target.FighterStats.Stats.WaterReduction + (pvp ? target.FighterStats.Stats.PvPWaterReduction : 0));
                    elementResistPercent = target.FighterStats.Stats.WaterResistPercent + (pvp ? target.FighterStats.Stats.PvPWaterResistPercent : 0);
                    break;
                case ElementType.Fire:
                    elementBonus = source.FighterStats.Stats.FireDamageBonus;
                    elementReduction = (short)(target.FighterStats.Stats.FireReduction  + (pvp ? target.FighterStats.Stats.PvPFireReduction : 0));
                    elementResistPercent = target.FighterStats.Stats.FireResistPercent + (pvp ? target.FighterStats.Stats.PvPFireResistPercent : 0);
                    break;
                case ElementType.Neutral:
                    elementBonus = source.FighterStats.Stats.NeutralDamageBonus;
                    elementReduction = (short)(target.FighterStats.Stats.NeutralReduction + (pvp? target.FighterStats.Stats.PvPNeutralReduction : 0));
                    elementResistPercent = target.FighterStats.Stats.NeutralResistPercent + (pvp ? target.FighterStats.Stats.PvPNeutralResistPercent : 0);
                    break;
                case ElementType.Push:
                    elementBonus = source.FighterStats.Stats.PushDamageBonus;
                    elementReduction = (short)(target.FighterStats.Stats.PushDamageReduction/* + (pvp ? target.FighterStats.Stats.PvP : 0)*/);
                    break;
                default:
                    break;
            }
            Delta += elementBonus;
            if (source.UsingWeapon)
                Delta += (short)((source.FighterStats.Stats.WeaponDamagesBonusPercent / 100) * Delta);
            if (elementResistPercent != 0)
                Delta = (short)(Delta - elementReduction - (elementResistPercent/ 100 * Delta));
            else
                Delta -= elementReduction;
            if (target.FighterStats.Stats.GlobalDamageReduction > 0 && ElementType != ElementType.Push)
            {//TODO evaluate properly damage reduction => the level should be the one of the dmg reduction spell's launcher, not the one of the player himself
                short reduction = (short)(target.FighterStats.Stats.GlobalDamageReduction * (100 + 5 * target.GetLevel()) / 100);
                Delta -= reduction;
                target.Fight.Send(new GameActionFightReduceDamagesMessage(0, source.ContextualId, target.ContextualId, (uint)reduction));
            }
            if (Delta <= 0)
                Delta = 0;
        }

    }
}
