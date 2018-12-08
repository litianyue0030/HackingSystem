using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace HackingSystem
{
    /// <summary>
    /// 行为基类
    /// </summary>
    /// <typeparam name="T">执行者类型</typeparam>
    public class GameAction<T> : ScriptableObject
    {
        /// <summary>
        /// 行为参数字典
        /// </summary>
        public Dictionary<string, object> ActionParams;

        virtual internal T Executor { get; set; }

        public virtual void Enter() { }

        public virtual void Execute() { }

        public virtual void Exit() { }
    }

    /// <summary>
    /// 行为列表的执行方式
    /// </summary>
    public enum ActionListExecuteMode
    {
        /// <summary>
        /// 循环执行
        /// </summary>
        loop,
        /// <summary>
        /// 单次线性执行
        /// </summary>
        line,
        /// <summary>
        /// 循环执行，执行后清除节点
        /// </summary>
        consume
    }

    public class RuleListEndConsume<T>:ExecuteRule
    {
        ActionList<T> list;
        public RuleListEndConsume(ActionList<T> List)
        {
            list = List;
        }
        public override bool RuleExecute()
        {
            return list.isNull;
        }
    }


    /// <summary>
    /// 行为列表，按列表一个一个执行行为
    /// </summary>
    /// <typeparam name="T">执行者类型</typeparam>
    [Serializable]
    public sealed class ActionList<T> : GameAction<T>
    {
        bool entered = false;
        bool NextTActive = false;

        ActionListExecuteMode mode;

        public ActionListExecuteMode executeMode
        {
            get { return mode; }
            set
            {
                mode = value;
                switch (value)
                {
                    case ActionListExecuteMode.loop:
                        next_type = Next_loop;
                        break;
                    case ActionListExecuteMode.line:
                        next_type = Next_line;
                        break;
                    case ActionListExecuteMode.consume:
                        next_type = Next_Consume;
                        break;
                }
            }
        }

        internal override T Executor
        {
            get
            {
                return base.Executor;
            }
            set
            {
                base.Executor = value;
                if (List.Count == 0)
                {
                    return;
                }
                foreach (var item in List)
                {
                    item.action.Executor = Executor;
                }
            }
        }

        LinkedList<ActionListNode<T>> List = new LinkedList<ActionListNode<T>>();

        LinkedList<ExecuteRule> rules = new LinkedList<ExecuteRule>();

        LinkedListNode<ActionListNode<T>> currentNode;

        LinkedListNode<ExecuteRule> currentRule;

        public ActionList(T executor)
        {
            base.Executor = executor;
        }

        public ActionList()
        {}

        public ActionListNode<T> AddAction_End(GameAction<T> action, ExecuteRule TransRule)
        {
            ActionListNode<T> a = new ActionListNode<T>();
            action.Executor = Executor;
            a._parent = this;
            a.action = action;
            List.AddLast(a);
            rules.AddLast(TransRule);
            if (currentNode == null)
            {
                if (entered)
                {
                    action.Enter();
                }
                currentNode = List.Last;
                currentRule = rules.Last;
            }

            return a;
        }

        public void Next()
        {
            next_type();
        }

        delegate void NextT();

        NextT next_type;

        void Next_Consume()
        {
            currentNode.Value.action.Exit();
            var c = currentNode.Next;
            var r = currentRule.Next;
            List.Remove(currentNode);
            rules.Remove(currentRule);
            if (List.Count == 0)
            {
                currentNode = null;
                currentRule = null;
            }
            if (c == null)
            {
                c = List.First;
                r = rules.First;
            }
            currentNode = c;
            currentRule = r;
            NextTActive = true;

        }

        void Next_loop()
        {
            currentNode.Value.action.Exit();
            var c = currentNode.Next;
            var r = currentRule.Next;
            if (c == null)
            {
                c = List.First;
                r = rules.First;
            }
            currentNode = c;
            currentRule = r;
            NextTActive = true;
        }

        void Next_line()
        {
            currentNode.Value.action.Exit();
            currentNode = currentNode.Next;
            currentRule = currentRule.Next;
            NextTActive = true;
        }

        public ActionListNode<T> AddAction_Next(GameAction<T> action, ExecuteRule TransRule)
        {
            ActionListNode<T> a = new ActionListNode<T>();
            a._parent = this;
            action.Executor = Executor;
            a.action = action;
            if (List.Count == 0)
            {
                if (entered)
                {
                    action.Enter();
                }
                List.AddLast(a);
                rules.AddLast(TransRule);
                currentNode = List.First;
                currentRule = rules.First;
            }
            else
            {
                List.AddAfter(List.First, a);
                rules.AddAfter(rules.First, TransRule);
            }
            return a;
        }

        public void Clear()
        {
            if (currentNode == null) return;
            if (entered)
            {
                currentNode.Value.action.Exit();
            }
            List.Clear();
            rules.Clear();
            currentNode = null;
            currentRule = null;
        }

        public override void Execute()
        {
            if (NextTActive)
            {
                NextTActive = false;
                if (currentNode != null)
                {
                    currentRule.Value.Reset();
                    currentNode.Value.action.Enter();
                }
            }
            if (currentNode == null) return;
            currentNode.Value.action.Execute();
            if (currentRule.Value)
            {
                Next();
            }
        }

        public override void Enter()
        {
            entered = true;
            if (List.Count != 0)
            {
                currentNode = List.First;
                currentRule = rules.First;
                currentRule.Value.Reset();
                currentNode.Value.action.Enter();

            }
        }

        public override void Exit()
        {
            if (currentNode != null)
            {
                currentNode.Value.action.Exit();
            }
        }

        public bool isNull
        {
            get { return List.Count == 0; }
        }

        public bool isEnd
        {
            get { return currentRule == null; }
        }
    }

    public sealed class ActionListNode<T>
    {
        internal ActionList<T> _parent;

        public GameAction<T> action;

        public string Name
        {
            get { return this.GetType().Name; }
        }

        //public abstract void XMLImport(XmlElement element);

        public ActionList<T> Parent
        {
            get { return _parent; }
        }

    }
       

    /// <summary>
    /// 规则
    /// </summary>
    public abstract class ExecuteRule : ScriptableObject
    {
        /// <summary>
        /// 规则执行
        /// </summary>
        /// <returns>执行结果是否符合该规则</returns>
        public abstract bool RuleExecute();

        /// <summary>
        /// 规则参数重置
        /// </summary>
        public virtual void Reset() { }
        
        public static implicit operator bool(ExecuteRule r)
        {
            return r.RuleExecute();
        }

        public static ExecuteRule operator &(ExecuteRule r1, ExecuteRule r2)
        {
            return new RuleAnd(r1, r2);
        }

        public static ExecuteRule operator |(ExecuteRule r1, ExecuteRule r2)
        {
            return new RuleOR(r1, r2);
        }

        public static ExecuteRule operator !(ExecuteRule r)
        {
            return new RuleNOT(r);
        }

        public static bool operator true(ExecuteRule rule)
        {
            return rule.RuleExecute();
        }

        public static bool operator false(ExecuteRule rule)
        {
            return !rule.RuleExecute();
        }
    }

    public sealed class RuleAnd : ExecuteRule
    {
        ExecuteRule L;
        ExecuteRule R;

        public override void Reset()
        {
            L.Reset();
            R.Reset();
        }

        public RuleAnd(ExecuteRule L, ExecuteRule R)
        {
            this.L = L;
            this.R = R;
        }
        public override bool RuleExecute()
        {
            return L.RuleExecute() && R.RuleExecute();
        }
    }

    public sealed class RuleOR : ExecuteRule
    {
        ExecuteRule L;
        ExecuteRule R;

        public override void Reset()
        {
            L.Reset();
            R.Reset();
        }

        public RuleOR(ExecuteRule L, ExecuteRule R)
        {
            this.L = L;
            this.R = R;
        }
        public override bool RuleExecute()
        {
            return L.RuleExecute() || R.RuleExecute();
        }
    }

    public sealed class RuleNOT : ExecuteRule
    {
        ExecuteRule R;

        public override void Reset()
        {
            R.Reset();
        }

        public RuleNOT(ExecuteRule R)
        {
            this.R = R;
        }
        public override bool RuleExecute()
        {
            return !R.RuleExecute();
        }
    }

    public sealed class RuleXOR : ExecuteRule
    {
        ExecuteRule L;
        ExecuteRule R;

        public override void Reset()
        {
            L.Reset();
            R.Reset();
        }

        public RuleXOR(ExecuteRule L, ExecuteRule R)
        {
            this.L = L;
            this.R = R;
        }
        public override bool RuleExecute()
        {
            return L.RuleExecute() != R.RuleExecute();
        }
    }

    public sealed class RuleEOR : ExecuteRule
    {
        ExecuteRule L;
        ExecuteRule R;

        public override void Reset()
        {
            L.Reset();
            R.Reset();
        }

        public RuleEOR(ExecuteRule L, ExecuteRule R)
        {
            this.L = L;
            this.R = R;
        }
        public override bool RuleExecute()
        {
            return L.RuleExecute() == R.RuleExecute();
        }
    }

    public sealed class FinateStateMachine<T> : GameAction<T>
    {
        internal override T Executor
        {
            get { return base.Executor; }
            set
            {
                base.Executor = value;
                foreach (var item in _stateList)
                {
                    item.Executor = value;
                }
            }
        }

        State<T>[] _stateList;

        public State<T>[] States
        {
            get { return _stateList; }
        }


        ExecuteRule[,] _stateTrans;

        State<T> _currentState;

        public State<T> CurrentState
        {
            get { return _currentState; }
            internal set
            {
                _currentState.action.Exit();
                _currentState = value;
                _currentState.action.Enter();
            }
        }

        /// <summary>
        /// 状态跳转对应的条件的邻接矩阵，其中 stateTrans[a,b]表示从state[a]跳转到state[b]的条件
        /// </summary>
        public ExecuteRule[,] StateTrans
        {
            get { return _stateTrans; }
        }

        /// <summary>
        /// 构造有限状态机，起始状态为StateList[0]
        /// </summary>
        /// <param name="stateList">状态机的状态集合</param>
        /// <param name="stateTrans">状态机的跳转条件集合，其中stateTrans[a,b]表示从state[a]跳转到state[b]的条件</param>
        public FinateStateMachine(GameAction<T>[] stateList, ExecuteRule[,] stateTrans, T executor)
        {
            _stateList = new State<T>[stateList.Length];

            _stateTrans = stateTrans;
            base.Executor = executor;
            for (int i = 0; i < stateList.Length; i++)
            {
                _stateList[i] = new State<T>();
                _stateList[i].action = stateList[i];
                _stateList[i].Executor = executor;
                _stateList[i].Parent = this;
                for (int j = 0; j < stateList.Length; j++)
                {
                    if (_stateTrans[i, j] != null)
                    {
                        _stateList[i].stateTrans.Add(_stateTrans[i, j]);
                        _stateList[i].TransState.Add(j);
                        _stateList[i].transCount++;
                    }
                }
            }

            _currentState = _stateList[0];
        }

        public override void Enter()
        {
            _currentState = States[0];
            _currentState.action.Enter();
            ResetTrans();
        }

        public override void Execute()
        {
            _currentState.action.Execute();
            if (_currentState.trans())
            {
                ResetTrans();
            }
        }

        private void ResetTrans()
        {
            for (int i = 0; i < _stateList.Length; i++)
            {
                for (int j = 0; j < _stateList.Length; j++)
                {
                    if (_stateTrans[i, j] != null)
                    {
                        _stateTrans[i, j].Reset();
                    }
                }
            }
        }

        public override void Exit()
        {
            _currentState.action.Exit();
        }
    }

    ///// <summary>
    ///// 子状态机
    ///// </summary>
    ///// <typeparam name="T">执行者类型</typeparam>
    //public class SubFinateStateMachine<T>:State<T>
    //{
    //    public override T Executor
    //    {
    //        get { return _executor; }
    //        internal set
    //        {
    //            _executor = value;
    //            for (int i = 0; i < _stateList.Length; i++)
    //            {
    //                _stateList[i].Executor = value;
    //            }
    //        }
    //    }

    //    State<T>[] _stateList;

    //    public State<T>[] States
    //    {
    //        get { return _stateList; }
    //    }

    //    /// <summary>
    //    /// 状态跳转对应的条件的邻接矩阵，其中 stateTrans[a,b]表示从state[a]跳转到state[b]的条件
    //    /// </summary>
    //    ExecuteRule[,] _stateTrans;

    //    State<T> _currentState;

    //    public State<T> CurrentState
    //    {
    //        get { return _currentState; }
    //        internal set
    //        {
    //            _currentState.Exit();
    //            _currentState = value;
    //            _currentState.Enter();
    //        }
    //    }

    //    public ExecuteRule[,] StateTrans
    //    {
    //        get { return _stateTrans; }
    //    }

    //    /// <summary>
    //    /// 构造状态机，起始状态为StateList[0]
    //    /// </summary>
    //    /// <param name="stateList">状态机的状态集合</param>
    //    /// <param name="stateTrans">状态机的跳转条件集合，其中stateTrans[a,b]表示从state[a]跳转到state[b]的条件</param>
    //    public SubFinateStateMachine(State<T>[] stateList, ExecuteRule[,] stateTrans)
    //    {
    //        _stateList = stateList;
    //        _stateTrans = stateTrans;
    //        for (int i = 0; i < stateList.Length; i++)
    //        {
    //            _stateList[i]._executor = _executor;
    //            _stateList[i].Parent = this;
    //            _stateList[i].SubF = true;
    //            for (int j = 0; j < stateList.Length; j++)
    //            {
    //                if (_stateTrans[i, j] != null)
    //                {
    //                    _stateList[i].stateTrans.Add(_stateTrans[i, j]);
    //                    _stateList[i].TransState.Add(j);
    //                    _stateList[i].transCount++;
    //                }
    //            }
    //        }
    //        _currentState = stateList[0];
    //    }

    //    public override void Enter()
    //    {
    //        _currentState.Enter();
    //    }

    //    public override void Execute()
    //    {
    //        _currentState.Execute();
    //        _currentState.trans();
    //    }

    //    public override void Exit()
    //    {
    //        _currentState.Exit();
    //    }
    //}

    /// <summary>
    /// 状态机状态
    /// </summary>
    /// <typeparam name="T">执行者类型</typeparam>
    public sealed class State<T>
    {
        public GameAction<T> action;

        public T Executor
        {
            get { return action.Executor; }
            internal set { action.Executor = value; }
        }
        internal FinateStateMachine<T> Parent;
        internal List<ExecuteRule> stateTrans = new List<ExecuteRule>();
        internal List<int> TransState = new List<int>();
        internal int transCount = 0;

        internal bool trans()
        {
            for (int i = 0; i < transCount; i++)
            {
                if (stateTrans[i])
                {
                    Parent.CurrentState = Parent.States[TransState[i]];
                    return true;
                }
            }
            return false;
        }
    }

    public delegate void BehaviorTreeEventHandler<T>(BehaviorTree<T> sender);

    /// <summary>
    /// 行为树
    /// </summary>
    /// <typeparam name="T">执行者类型</typeparam>
    public sealed class BehaviorTree<T> : GameAction<T>
    {
        BTNode<T> root;

        public BTNode<T> Root { get { return root; } }

        BehaviorExecuteState PrevResult;

        internal override T Executor
        {
            get
            {
                return base.Executor;
            }

            set
            {
                base.Executor = value;
                root.Executor = value;
            }
        }

        public BehaviorTree(BTNode<T> root, T executor)
        {
            this.root = root;
            Executor = executor;
            root.Owner = this;
        }

        public override void Enter()
        {
            Root.Reset();
            Root.ExecuteState = BehaviorExecuteState.executing;
        }

        public override void Execute()
        {
            if (Root.ExecuteState == BehaviorExecuteState.failed || Root.ExecuteState == BehaviorExecuteState.finishied)
            {
                PrevResult = Root.ExecuteState;

                if (RoundExecuted != null)
                {
                    RoundExecuted(this);
                }
                Root.Reset();
                return;
            }
            if (Root.ExecuteState == BehaviorExecuteState.Ready)
            {
                Root.ExecuteState = BehaviorExecuteState.executing;
            }
            Root.Execute();
        }

        /// <summary>
        /// 完成一轮的执行后发生
        /// </summary>
        public event BehaviorTreeEventHandler<T> RoundExecuted;

        public override void Exit()
        {
            Root.InterruptAndExit();
        }
    }

    /// <summary>
    /// 行为树节点的执行情况
    /// </summary>
    public enum BehaviorExecuteState
    {
        finishied,
        executing,
        failed,
        Ready
    }

    public abstract class BTNode<T>
    {
        T executor;
        internal virtual T Executor
        {
            get { return executor; }
            set
            {
                executor = value;
                foreach (var item in subNodes)
                {
                    item.executor = value;
                }
            }
        }

        public abstract void Execute();

        BehaviorExecuteState executeState = BehaviorExecuteState.Ready;
        public virtual BehaviorExecuteState ExecuteState
        {
            get { return executeState; }
            protected internal set
            {
                executeState = value;

            }
        }

        /// <summary>
        /// 将完成任务该节点重置为Ready
        /// </summary>
        public virtual void Reset()
        {
            if (executeState == BehaviorExecuteState.executing)
            {
                throw new Exception("试图重置执行中的行为");
            }
            if (SubNodes.Count == 0)
            {
                ExecuteState = BehaviorExecuteState.Ready;
                return;
            }
            ExecuteState = BehaviorExecuteState.Ready;
            foreach (var item in SubNodes)
            {
                item.Reset();
            }
        }

        /// <summary>
        /// 终止节点的执行并退出，执行结果记为Failed
        /// </summary>
        public void InterruptAndExit()
        {
            if (ExecuteState != BehaviorExecuteState.executing)
            {
                return;
            }
            ExecuteState = BehaviorExecuteState.failed;
            if (SubNodes.Count == 0) return;
            foreach (var item in SubNodes)
            {
                item.InterruptAndExit();
            }
        }

        BTNode<T> parent;

        /// <summary>
        /// 父节点
        /// </summary>
        public BTNode<T> ParentNode
        {
            get { return parent; }
            internal set { parent = value; }
        }

        protected BehaviorTree<T> owner;

        /// <summary>
        /// 节点持有者，修改时会自动应用到子节点
        /// </summary>
        public virtual BehaviorTree<T> Owner
        {
            get { return owner; }
            internal set
            {
                owner = value;
                foreach (var item in subNodes)
                {
                    item.Owner = value;
                }
            }
        }

        LinkedList<BTNode<T>> subNodes = new LinkedList<BTNode<T>>();

        /// <summary>
        /// 刷新节点的执行者，父节点，持有者等数据,并应用到所有子节点
        /// </summary>
        protected virtual void DataRefresh()
        {
            foreach (var item in subNodes)
            {
                item.owner = owner;
                item.parent = this;
                item.DataRefresh();
            }
        }

        public LinkedList<BTNode<T>> SubNodes
        {
            get { return subNodes; }
        }
    }

    public sealed class BTSelectNode<T> : BTNode<T>
    {
        public LinkedList<ExecuteRule> rules;

        LinkedListNode<BTNode<T>> curNode;

        public override void Execute()
        {
            bool executing = false;
            bool successful = false;
            for (; curNode != null;)
            {
                if (curNode.Value.ExecuteState == BehaviorExecuteState.failed)
                {
                    curNode = curNode.Next;
                    if (curNode == null)
                    {
                        successful = false;
                    }
                }
                else if (curNode.Value.ExecuteState == BehaviorExecuteState.finishied)
                {
                    successful = true;
                    break;
                }
                else
                {
                    executing = true;
                    if (curNode.Value.ExecuteState == BehaviorExecuteState.Ready)
                    {
                        curNode.Value.ExecuteState = BehaviorExecuteState.executing;
                    }
                    curNode.Value.Execute();
                    break;
                }
            }
            if (executing) return;
            if (!successful)
            {
                ExecuteState = BehaviorExecuteState.failed;
            }
            else
            {
                ExecuteState = BehaviorExecuteState.finishied;
            }
        }

        public override void Reset()
        {
            base.Reset();
            curNode = SubNodes.First;
        }

    }

    public enum ParallelNodeFinishMode
    {
        AllClear,
        oneFinish
    }

    public sealed class BTParallelNode<T> : BTNode<T>
    {

        /// <summary>
        /// 成功执行的判定标准,单个完成还是全部完成
        /// </summary>
        public ParallelNodeFinishMode mode;

        public BTParallelNode(ParallelNodeFinishMode mode)
        {
            this.mode = mode;
        }

        public override void Execute()
        {
            bool allClear = true;
            bool oneFinished = false;
            bool executing = false;
            foreach (var item in SubNodes)
            {
                if (item.ExecuteState == BehaviorExecuteState.finishied)
                {
                    oneFinished = true;
                    continue;
                }
                if (item.ExecuteState == BehaviorExecuteState.failed)
                {
                    allClear = false;
                    continue;
                }

                if (item.ExecuteState == BehaviorExecuteState.Ready)
                {
                    item.ExecuteState = BehaviorExecuteState.executing;
                }
                item.Execute();
                executing = true;
            }
            if (!executing)
            {
                if (mode == ParallelNodeFinishMode.AllClear)
                {
                    ExecuteState = allClear ? BehaviorExecuteState.finishied : BehaviorExecuteState.failed;
                }
                else
                {
                    ExecuteState = oneFinished ? BehaviorExecuteState.finishied : BehaviorExecuteState.failed;
                }
            }
        }
    }

    public sealed class BTSequenceNode<T> : BTNode<T>
    {
        LinkedListNode<BTNode<T>> n;

        public override void Reset()
        {
            base.Reset();
            n = SubNodes.First;
        }

        public override void Execute()
        {
            if (n.Value.ExecuteState == BehaviorExecuteState.finishied)
            {
                if (n.Next == null)
                {
                    ExecuteState = BehaviorExecuteState.finishied;
                }
                else
                {
                    n = n.Next;
                }
            }
            else if (n.Value.ExecuteState == BehaviorExecuteState.failed)
            {
                ExecuteState = BehaviorExecuteState.failed;
                return;
            }
            else
            {
                if (n.Value.ExecuteState == BehaviorExecuteState.Ready)
                {
                    n.Value.ExecuteState = BehaviorExecuteState.executing;
                }

                n.Value.Execute();
            }

        }
    }

    public sealed class BTActionNode<T> : BTNode<T>
    {
        public GameAction<T> action;

        public ExecuteRule FailRule;

        public ExecuteRule SucceedRule;

        internal override T Executor
        {
            get
            {
                return base.Executor;
            }

            set
            {
                base.Executor = value;
                action.Executor = value;
            }
        }

        public override BehaviorExecuteState ExecuteState
        {
            get
            {
                return base.ExecuteState;
            }

            protected internal set
            {

                if (base.ExecuteState == BehaviorExecuteState.Ready && value == BehaviorExecuteState.executing)
                {
                    action.Enter();
                    FailRule.Reset();
                    SucceedRule.Reset();
                }
                if (base.ExecuteState == BehaviorExecuteState.executing && (value == BehaviorExecuteState.failed || value == BehaviorExecuteState.finishied))
                {
                    action.Exit();
                }
                base.ExecuteState = value;
            }
        }

        public override BehaviorTree<T> Owner
        {
            get
            {
                return owner;
            }

            internal set
            {
                owner = value;
                action.Executor = owner.Executor;
            }
        }

        public BTActionNode(GameAction<T> action, ExecuteRule failRule, ExecuteRule succeedRule)
        {
            this.action = action;
            FailRule = failRule;
            SucceedRule = succeedRule;
        }

        public override void Execute()
        {
            if (FailRule)
            {
                ExecuteState = BehaviorExecuteState.failed;
            }
            else if (SucceedRule)
            {
                ExecuteState = BehaviorExecuteState.finishied;
            }
            action.Execute();
        }

        protected override void DataRefresh()
        {
            base.DataRefresh();
            action.Executor = owner.Executor;
        }
    }

    /// <summary>
    /// 妖梦新增的随机节点，通过随机选择函数决定执行的子节点，并且将子节点的执行情况反映为节点执行情况，先执行后反馈
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class BTRandomNode<T> : BTNode<T>
    {
        int rand = -1;

        LinkedListNode<BTNode<T>> curNode;

        public BTRandomNode(RandomSelector s)
        {
            st = s;
        }

        public delegate int RandomSelector(T executor);

        RandomSelector st;

        public override void Reset()
        {
            base.Reset();
            rand = -1;
        }

        public override void Execute()
        {
            if (rand == -1)
            {
                curNode = SubNodes.First;
                rand = st(owner.Executor);
                for (int i = 0; i < rand; i++)
                {
                    curNode = curNode.Next;
                }
                curNode.Value.ExecuteState = BehaviorExecuteState.executing;
            }
            curNode.Value.Execute();
            ExecuteState = curNode.Value.ExecuteState;
        }
    }

    public abstract class Utility
    {
        public abstract float ResultCalc();
    }

    public sealed class UtilityAction<T> : GameAction<T>
    {
        List<GameAction<T>> actionList = new List<GameAction<T>>();
        List<Utility> actionScore = new List<Utility>();
        List<ExecuteRule> actionTrans = new List<ExecuteRule>();

        int maxId;

        /// <summary>
        /// 增加一个决策行为
        /// </summary>
        /// <param name="action">行为</param>
        /// <param name="actionScoreCalc">行为决策函数</param>
        /// <param name="actionTrans">行为跳转的规则</param>
        public void addAction(GameAction<T> action, Utility actionScoreCalc, ExecuteRule actionTrans)
        {
            actionList.Add(action);
            actionScore.Add(actionScoreCalc);
            this.actionTrans.Add(actionTrans);
        }

        public override void Enter()
        {
            float MaxScore = float.MinValue;
            maxId = 0;
            for (int i = 0; i < actionList.Count; i++)
            {
                if (actionScore[i].ResultCalc() > MaxScore)
                {
                    MaxScore = actionScore[i].ResultCalc();
                    maxId = i;
                }
            }
            actionList[maxId].Enter();
        }

        public override void Execute()
        {
            actionList[maxId].Execute();
            if (actionTrans[maxId])
            {
                actionList[maxId].Exit();
                Enter();
            }
        }

        public override void Exit()
        {
            actionList[maxId].Exit();
        }
    }

    public abstract class Selecter
    {
        public abstract int ExecuteSelecter();
    }

    public sealed class RuleAction<T> : GameAction<T>
    {
        List<GameAction<T>> actionList = new List<GameAction<T>>();
        Selecter selecter;
        List<ExecuteRule> actionTrans = new List<ExecuteRule>();
        public ExecuteRule curRule;
        public bool loop = false;
        
        public int curID;

        internal override T Executor
        {
            get
            {
                return base.Executor;
            }
            set
            {
                base.Executor = value;
                foreach (var item in actionList)
                {
                    item.Executor = Executor;
                }
            }
        }

        /// <param name="Selecter">选择规则</param>
        public RuleAction(Selecter Selecter)
        {
            selecter = Selecter;
        }

        /// <summary>
        /// 增加一个决策行为
        /// </summary>
        /// <param name="action">行为</param>
        /// <param name="actionTrans">行为跳转的规则</param>
        public void addAction(GameAction<T> action, ExecuteRule actionTrans)
        {
            actionList.Add(action);
            this.actionTrans.Add(actionTrans);
        }

        public override void Enter()
        {
            curID = selecter.ExecuteSelecter();
            actionList[curID].Enter();
            curRule = actionTrans[curID];
        }

        public override void Execute()
        {
            if (curID == -1 && loop)
            {
                Enter();
            }
            actionList[curID].Execute();
            if (actionTrans[curID])
            {
                actionList[curID].Exit();
                curID = -1;
            }
        }

        public override void Exit()
        {
            if (curID != -1)
            {
                actionList[curID].Exit();
            }
        }
    }

    public sealed class RuleActionTrans<T> : ExecuteRule
    {
        RuleAction<T> action;
        public RuleActionTrans(RuleAction<T> action)
        {
            this.action = action;
        }
        public override bool RuleExecute()
        {
            if (action.curRule)
            {
                UnityEngine.Debug.Log("RuleActionEnd");
                return true;
            }
            return action.curRule;
        }
    }
}