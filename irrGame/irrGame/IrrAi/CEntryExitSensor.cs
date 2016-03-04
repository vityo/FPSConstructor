using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrGame.IrrAi.Interface;

using IrrlichtLime.Scene;

namespace IrrGame.IrrAi
{
    public class CEntryExitSensor : IAISensor 
    {
        public enum E_AISENSOR_STATE_TYPE 
        {
			EAISST_INSIDE,
			EAISST_OUTSIDE,
			E_NUM_AISENSOR_STATE_TYPES
		}

		protected class SEntryExitSensorData : SSensorData 
        {
			public E_AISENSOR_STATE_TYPE State;

            public SEntryExitSensorData(){}
		}

		public CEntryExitSensor(SAIEntityDesc desc, IAIManager aimgr, SceneManager smgr, int id):
            base(desc, aimgr, smgr, E_AIENTITY_TYPE.EAIET_ENTRYEXIT_SENSOR, id)
        {}

        public void update(uint elapsedTime)
        {
	        base.update(elapsedTime);

	        if (cptr==null)
                return;
  
	        for (int i = 0 ; i < Entities.Count ; ++i) 
            {
		        IAIEntity entity = Entities[i].Entity;

                //????: intersects
		        if (entity.getNode().BoundingBoxTransformed.IsInside(Node.BoundingBoxTransformed)) 
                {
			        if (((SEntryExitSensorData)Entities[i]).State == E_AISENSOR_STATE_TYPE.EAISST_OUTSIDE) 
				        cptr(this, entity, E_AISENSOR_EVENT_TYPE.EAISET_ENTER);

			        ((SEntryExitSensorData)Entities[i]).State = E_AISENSOR_STATE_TYPE.EAISST_INSIDE;
		        }
                else
                {
			        if (((SEntryExitSensorData)Entities[i]).State == E_AISENSOR_STATE_TYPE.EAISST_INSIDE) 
				        cptr(this, entity, E_AISENSOR_EVENT_TYPE.EAISET_EXIT);
			
                    ((SEntryExitSensorData)Entities[i]).State = E_AISENSOR_STATE_TYPE.EAISST_OUTSIDE;
		        }
            }
        }

        public void addEntity(IAIEntity entity)
        {
	        if (entity==null)
                return;

	        SEntryExitSensorData data = new SEntryExitSensorData();
	
            data.Entity = entity;

	        if (entity.getNode().BoundingBoxTransformed.IsInside(Node.BoundingBoxTransformed))
		        data.State = E_AISENSOR_STATE_TYPE.EAISST_INSIDE;
	        else
		        data.State = E_AISENSOR_STATE_TYPE.EAISST_OUTSIDE;

	        Entities.Add(data);
        }
    }
}
