using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

using IrrlichtLime.Scene;

namespace IrrGame.IrrAi
{
    public class CPlayerAIEntity : IPlayerAIEntity 
    {
        public CPlayerAIEntity(SAIEntityDesc desc, IAIManager aimgr, SceneManager smgr, int id) :
            base(desc, aimgr, smgr, id) 
        {}

                          
    }
}
