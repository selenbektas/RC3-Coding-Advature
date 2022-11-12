using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public abstract class VolumetricMask : MonoBehaviour
{
    public abstract bool IsMaskedTrue(Vector3 point);

}
