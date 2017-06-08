using Symbioz.DofusProtocol.Messages;
using Symbioz.DofusProtocol.Types;
using Symbioz.Enums;
using Symbioz.World.Models.Fights.Fighters;
using Symbioz.World.Records.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.Providers.SpellEffectsProvider.Buffs
{
    public abstract class Buff
    {
        public Fighter Fighter { get; set; }
        public uint UID { get; set; }
        public short Delta { get; set; }
        public short Duration { get; set; }
        public int SourceId { get; set; }
        public short SourceSpellId {
            get
            {
                return (short)SpellLevelRecord.GetLevel(this.SourceSpellLevelId).SpellId;
            }
        }
        public short SourceSpellLevelId { get; set; }
        public int Delay { get; set; }
        public virtual FighterEventType EventType { get { return FighterEventType.ON_CASTED; } }

        public Buff(uint UID,short delta,short duration,int sourceid,short sourcespelllevelid,int delay)
        {
            this.Delta = delta;
            this.Duration = duration;
            this.SourceId = sourceid;
            this.SourceSpellLevelId = sourcespelllevelid;
            this.UID = UID;
            this.Delay = delay;

        }
        public abstract void SetBuff();

        public abstract void RemoveBuff();

        public virtual bool OnEventCalled(object arg1,object arg2,object arg3) { return false; }

        public virtual void OnBuffAdded() { }
        public void Initialize(Fighter fighter)
        {
            this.Fighter = fighter;
        }
        public GameActionFightDispellableEffectMessage DefaultMessage(EffectsEnum effect)
        {
            return new GameActionFightDispellableEffectMessage((ushort)effect, SourceId, new FightTemporaryBoostEffect((uint)Fighter.BuffIdProvider.Pop(), Fighter.ContextualId, Duration,1, (ushort)SourceSpellId, (uint)effect, 16, Delta));
        }

        public void setDuration(int value, Fighter modifier)
        {
            this.Duration = (short)value;
        }
    }
}
