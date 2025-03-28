using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Used to Create a primitive object to be used by the NPC.")]
    public class PrimitiveCreate : EnemyAction
    {
        public SharedVariable<GameObject> primitiveReference;
        public string nameOfPrimitive = "Rotation-Look-At";
        // Start is called before the first frame update
        public override void OnStart()
        {
            TryCreatePrimitive();
        }

        private void TryCreatePrimitive()
        {
            if (primitiveReference == null || primitiveReference.Value == null)
                CreatePrimitive();
        }

        private void CreatePrimitive()
        {
            float scaleSize = 0.25f;
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //primitiveLookAt.transform.parent = this.transform;
            obj.name = "Rotation-Look-At";
            obj.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
            var ren = obj.GetComponent<MeshRenderer>();
            ren.enabled = false;
            var col = obj.GetComponent<Collider>();
            col.enabled = false;
            obj.name = string.Format("{0}_{1}", nameOfPrimitive, this.transform.parent.name);

            //if this NPC object also has a parent. like a parent Group. lets also make it 'this' primitives parent.
            if (this.transform.parent.parent != null)
                obj.transform.SetParent(this.transform.parent.parent);

            primitiveReference.Value = obj;
        }
    }
}