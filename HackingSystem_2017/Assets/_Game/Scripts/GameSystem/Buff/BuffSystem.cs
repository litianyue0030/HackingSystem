using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HackingSystem
{
    public class BuffSystem
    {
        LinkedList<Buff> buffs;
        LinkedList<Buff> debuffs;
        public Bot owner
        {
            get;set;
        }
        
        public BuffSystem()
        {
            buffs = new LinkedList<Buff>();
            debuffs = new LinkedList<Buff>();
        }

        // Update is called once per frame
        public void Update()
        {
            LinkedListNode<Buff> buffNode;
            for (buffNode = buffs.First; buffNode != null; )
            {
                buffNode.Value.Execute();
                
                if (buffNode.Value.BuffEndRule)
                {
                    buffNode.Value.Exit();
                    if (buffNode == buffs.First)
                    {
                        buffs.RemoveFirst();
                        buffNode = buffs.First;
                        continue;
                    }
                    else
                    {
                        var nodeDelete = buffNode;
                        buffNode = buffNode.Previous;
                        buffs.Remove(nodeDelete);
                    }
                }
                buffNode = buffNode.Next;
            }

            LinkedListNode<Buff> debuffNode;
            for (debuffNode = debuffs.First; debuffNode != null;)
            {
                debuffNode.Value.Execute();
                if (debuffNode.Value.BuffEndRule)
                {
                    debuffNode.Value.Exit();
                    if (debuffNode == debuffs.First)
                    {
                        debuffs.RemoveFirst();
                        debuffNode = debuffs.First;
                        continue;
                    }
                    else
                    {
                        var denodeDelete = debuffNode;
                        debuffNode = debuffNode.Previous;
                        debuffs.Remove(denodeDelete);
                    }
                }
                debuffNode = debuffNode.Next;
            }
        }
        
        /// <summary>
        /// 添加不可重复叠加Buff
        /// </summary>
        /// <param name="B">要添加的Buff</param>
        public void AddBuff(Buff B)
        {
            //重叠（施术者相同且Buff类型相同）
            BuffType buffType = B.BuffType;
            if (buffType == BuffType.buff)
            {
                for (var node = buffs.First; node != null; node = node.Next)
                {
                    if (node.Value.BuffEquals(B))
                    {
                        if (node.Value.Level <= B.Level)
                        {
                            buffs.Remove(node);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                for (var node = debuffs.First; node != null; node = node.Next)
                {
                    if (node.Value.GetType() == B.GetType() && node.Value.Source == B.Source)
                    {
                        if (node.Value.Level <= B.Level)
                        {
                            debuffs.Remove(node);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }

            //增益还是减益
            if (B.BuffType == BuffType.buff)
            {
                buffs.AddLast(B);
            }
            else if (B.BuffType == BuffType.debuff)
            {
                debuffs.AddLast(B);
            }
            B.BuffEndRule.Reset();
            B.Enter();
        }
        
        /// <summary>
        /// 添加可重复叠加Buff
        /// </summary>
        /// <param name="B">要添加的Buff</param>
        public void AddBuffMult(Buff B)
        {
            if (B.BuffType == BuffType.buff)
            {
                buffs.AddLast(B);
            }
            else if (B.BuffType == BuffType.debuff)
            {
                debuffs.AddLast(B);
            }
            B.BuffEndRule.Reset();
            B.Enter();
        }

        /// <summary>
        /// 直接解除某个Buff
        /// </summary>
        /// <param name="B">要解除的Buff</param>
        public void BuffDecast(Buff B)
        {
            switch (B.BuffType)
            {
                case BuffType.buff:
                    if(buffs.Remove(B))
                    {
                        B.Exit();
                    }
                    break;
                case BuffType.debuff:
                    if(debuffs.Remove(B))
                    {
                        B.Exit();
                    }
                    break;
            }
        }

        /// <summary>
        /// 解除所有增益
        /// </summary>
        public void BuffDeCastAll()
        {
            foreach (var item in buffs)
            {
                item.Exit();
            }
            buffs.Clear();
        }

        /// <summary>
        /// 解除所有减益
        /// </summary>
        public void DebuffDecastAll()
        {
            foreach (var item in debuffs)
            {
                item.Exit();
            }
            debuffs.Clear();
        }
    }

    public abstract class Buff
    {
        public string BuffName;

        public Buff()
        {
            BuffName = GetType().ToString();
        }

        public BuffType BuffType
        {
            get;protected set;
        }
        public Bot Source
        {
            get;set;
        }
        public BuffSystem Owner
        {
            get;set;
        }
        public ExecuteRule BuffEndRule
        {
            get;set;
        }
        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();

        /// <summary>
        /// 名字相同且施术者相同为相同的Buff
        /// </summary>
        public bool BuffEquals(Buff obj)
        {
            return obj.BuffName .Equals(BuffName) && Source == obj.Source;
        }

        /// <summary>
        /// Buff覆盖等级，在重复Buff附加时，高等级的Buff会覆盖低等级的Buff，等级相同则新Buff覆盖旧Buff
        /// </summary>
        public int Level
        { get; set; }
    
    }

    public enum BuffType
    {
        buff,
        debuff
    }
}