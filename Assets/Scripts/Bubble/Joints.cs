using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Фабрика-хранилище для упругих связей пузыря.
/// </summary>
public class Joints
{
    /// <summary>
    /// Словарь связей, ключём является тело к которому эта связь прикреплена.
    /// </summary>
    private Dictionary<Rigidbody2D, SpringJoint2D> neighborsJoints;

    /// <summary>
    /// Список связей с землёй;
    /// </summary>
    private List<SpringJoint2D> groundJoints;

    /// <summary>
    /// Хранилище связей.
    /// </summary>
    private Stack<SpringJoint2D> _storeSprings;

    /// <summary>
    /// Объект владелец связей.
    /// </summary>
    private Bubble _owner;

    public Joints(Bubble owner)
    {
        _owner = owner;
        _storeSprings = new Stack<SpringJoint2D>(4);
        _owner.NeedDestroy += (arg) => BreakAllJoint();
    }

    /// <summary>
    /// Создание связи. Если до этого объект содержал связи, то они достаются из хранилища.
    /// </summary>
    /// <param name="connectedBody"></param>
    /// <returns></returns>
    private SpringJoint2D CreateJoint(Rigidbody2D connectedBody)
    {
        SpringJoint2D joint = GetJoint();

        joint.connectedBody = connectedBody;
        joint.connectedAnchor = Vector2.zero;
        joint.dampingRatio = 1;
        joint.enableCollision = true;

        joint.autoConfigureDistance = false;
        joint.frequency = 10f;
        joint.distance = 2*_owner.Collider.radius;
        return joint;
    }

    private SpringJoint2D GetJoint()
    {
        SpringJoint2D joint;
        if (_storeSprings.Count > 0)
        {
            joint = _storeSprings.Pop();
            joint.enabled = true;
        }
        else
        {
            joint = _owner.gameObject.AddComponent<SpringJoint2D>();
        }

        return joint;
    }

    /// <summary>
    /// Создание связи к земле. Если до этого объект содержал связи, то они достаются из хранилища.
    /// </summary>
    /// <param name="connectedBody"></param>
    /// <returns></returns>
    private SpringJoint2D CreateJointInGround()
    {
        SpringJoint2D joint = GetJoint();
        
        joint.autoConfigureDistance = false;
        joint.frequency = 10f;
        joint.distance = 0f;

        joint.autoConfigureConnectedAnchor = true;
        joint.dampingRatio = 1;
        joint.enableCollision = true;
        return joint;
    }

    private void BreakJoint(Rigidbody2D rb)
    {
        if (neighborsJoints.ContainsKey(rb))
        {
            _storeSprings.Push(neighborsJoints[rb]);
            neighborsJoints[rb].enabled = false;
            neighborsJoints[rb].connectedBody = null;
            neighborsJoints.Remove(rb);
            //        Destroy(neighborsJoints[rb]);
        }
    }

    private void BreakGroundsJoint()
    {
        if (groundJoints != null)
        {
            foreach (var joint in groundJoints)
            {
                _storeSprings.Push(joint);
                joint.enabled = false;
                joint.connectedBody = null;
            }
            groundJoints.Clear();
        }
    }


    public void BreakAllJoint()
    {
        foreach (var items in neighborsJoints)
        {
            _storeSprings.Push(items.Value);
            items.Value.enabled = false;
            items.Value.connectedBody = null;
        }
        BreakGroundsJoint();
        neighborsJoints.Clear();
    }

    /// <summary>
    /// Создание новых связей между владельцем и списком пузырей.
    /// </summary>
    /// <param name="bubbles"></param>
    public void CreateSpringJoints(IEnumerable<Bubble> bubbles)
    {
        neighborsJoints = new Dictionary<Rigidbody2D, SpringJoint2D>(bubbles.Count());

        foreach (Bubble bubble in bubbles)
        {
            if (bubble)
            {
                var rb = bubble.RigidBody;
                if (_owner.RigidBody != rb)
                {
                    neighborsJoints.Add(rb, CreateJoint(rb));
                    bubble.BurstStart += BreakJoint;
                }
            }
            else
            {
                if (groundJoints == null)
                    groundJoints = new List<SpringJoint2D>(2);
                groundJoints.Add(CreateJointInGround());
            }
        }
    }

    /// <summary>
    /// Регистрация новых пружин по списку прикреплённых к этому объекту пружин.
    /// </summary>
    /// <param name="joints"></param>
    public void RegistrationSpringJoints(IEnumerable<SpringJoint2D> joints)
    {
        neighborsJoints = new Dictionary<Rigidbody2D, SpringJoint2D>(joints.Count());

        foreach (SpringJoint2D joint in joints)
        {
            var rb = joint.attachedRigidbody;
            if (rb != null)
            {
                neighborsJoints.Add(rb, joint);

                joint.gameObject.GetComponent<Bubble>().BurstStart += BreakJoint;
            }
        }
    }
}
