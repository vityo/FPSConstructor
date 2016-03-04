using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime.Scene;

namespace IrrGame.IrrAi.Interface
{
    public class IPlayerAIEntity : IAIEntity 
    {
		public IPlayerAIEntity(SAIEntityDesc desc, IAIManager aimgr, SceneManager smgr, int id) 
            : base(desc, aimgr, smgr, E_AIENTITY_TYPE.EAIET_PLAYER, id) 
        {}
    }
}
