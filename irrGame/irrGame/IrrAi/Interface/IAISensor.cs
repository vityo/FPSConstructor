using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime.Scene;

namespace IrrGame.IrrAi.Interface
{
    public enum E_AISENSOR_EVENT_TYPE 
    {
	    EAISET_ENTER,
	    EAISET_EXIT,
	    E_NUM_AISENSOR_EVENT_TYPES
    }

    public class IAISensor : IAIEntity 
    {
        protected class SSensorData 
        {
            public IAIEntity Entity;
			public SSensorData()
            {
				Entity = null;
			}
		}

		protected List<SSensorData> Entities;      
          
		public delegate void CallbackPtr(IAISensor iaisensor,IAIEntity iaientity ,E_AISENSOR_EVENT_TYPE eaet);
        public CallbackPtr cptr;

		public IAISensor(SAIEntityDesc desc, IAIManager aimgr, SceneManager smgr, E_AIENTITY_TYPE type, int id) : 
            base(desc, aimgr, smgr, type, id) 
        {
			cptr = null;
		}

		~IAISensor() 
        {
			Entities.Clear();
		}

		public virtual void setCallback(CallbackPtr cb)
        {
            cptr = cb; 
        }

		public virtual void addEntity(IAIEntity entity) 
        {
			if (entity==null)
                return;

			SSensorData data = new SSensorData();
			data.Entity = entity;
			Entities.Add(data);
		}

		public virtual void removeEntity(IAIEntity entity) 
        {
			for (int i = 0 ; i < Entities.Count ; ++i)
				if (Entities[i].Entity == entity) 
                {
					Entities.RemoveAt(i);

					break;
				}
		}
    }
}
