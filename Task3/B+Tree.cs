using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using System.IO;
namespace Task3
{
     
    class Node
    {
        public int MaxOrder;
        public ChildNode Parent;
     
    }
    class Record
    {
        public string Key;
        public List<string> Datas;
       public Record(string Key,List<string> Datas)
        {
            this.Key = Key;
            this.Datas = Datas;
        }
    }
    class  LeafNode:Node
    {
        public List<Record> Records;
        public LeafNode Next;
     
        
        public LeafNode(int MaxOrder)
        {
            this.MaxOrder = MaxOrder;
            Records = new List<Record>();
            Parent = null;
            Next = null;
        }
        public void RecordsSort()
        {
            Records.Sort((a,b)=> { return int.Parse( a.Key).CompareTo(int.Parse(b.Key)); });
        }
       
        public LeafNode(int MaxOrder,List<Record> records)
        {
            this.MaxOrder = MaxOrder;
            Records = new List<Record>();
            Parent = null;
            Next = null;

            for (int i = 0; i < records.Count; i++)
            {
                Records.Add(records[i]);
            }
            RecordsSort();
        }
        public void DeleteRecord(string key,ref ChildNode rootnode)
        {
            Records.Remove(Records.Find((a) => { return int.Parse(a.Key) == int.Parse(key); }));
            LeafNode Last = null;
            int amount = (int)(Math.Ceiling(((float)MaxOrder / 2)) - 1);
            if (Records.Count >= amount)
            {
                if (Parent.Keys.Contains(key))
                    Parent.Delete(key, ref rootnode);
                return;
            }
          
            if (Parent.Nodes.FindIndex((a) => { return a == this; })>0)
                {
                Last = Parent.Nodes[Parent.Nodes.FindIndex((a) => { return a == this; }) - 1] as LeafNode;
                if (Last.Records.Count > amount)
                {
                    Records.Add(Last.Records[0]);
                    Last.Records.RemoveAt(0);
                    RecordsSort();
                      return;
                }
                }


                if (Next != null)
                {
                    if (Next.Records.Count > amount)
                    {
                        Records.Add(Next.Records[0]);
                        Next.Records.RemoveAt(0);
                    RecordsSort();
                   
                    if (Parent.Keys.Contains(key))
                        Parent.Delete(key,ref rootnode);
                    return;
                    }
                }



                if (Records.Count < amount)
                {

                   if(Last!=null)
                {
                    for (int i = 0; i < Records.Count; i++)
                    {
                        Last.Records.Add(Records[i]);
                    }
                    Last.RecordsSort();
                    Records.Clear();
                    Parent.Nodes.Remove(this);
                    Last.Next = Next;
                    if (Parent.Keys.Contains(key))
                     Parent.Delete(key,ref rootnode);
                    return;
                }

                 

                }
             

        }
       public void InsertRecord(Record record,ref ChildNode RootNode)
        {
            Records.Add(record);
            RecordsSort();
            
            if(Records.Count>= MaxOrder)
            {
                RootNode = Split();
            }
            
        }
        
        public ChildNode Split()
        {
            if(Parent==null)
            {
                ChildNode node = new ChildNode(MaxOrder);
                node.Keys.Add(Records[MaxOrder / 2].Key);
                List<Record> NextRecord = new List<Record>();
                for (int i = MaxOrder / 2; i < MaxOrder; i++)
                {  
                    NextRecord.Add(Records[i]);
                   
                }
                
                    
                 Records.RemoveRange(MaxOrder / 2, MaxOrder - MaxOrder / 2);
                 
                LeafNode NextNode = new LeafNode(MaxOrder,NextRecord);
                this.Next = NextNode;
               
                this.Parent = node;
                NextNode.Parent = node;
                node.Nodes.Add(this);
                node.Nodes.Add(NextNode);
                Parent.SortKey();
             
            }
            else
            {
                Parent.Keys.Add(Records[MaxOrder / 2].Key);
                List<Record> NextRecord = new List<Record>();
                for (int i = MaxOrder / 2; i < MaxOrder; i++)
                {
                    NextRecord.Add(Records[i]);
                     
                }
               
                Records.RemoveRange(MaxOrder / 2, MaxOrder - MaxOrder / 2);
                LeafNode NextNode = new LeafNode(MaxOrder,NextRecord);
                
                NextNode.Next = Next;
                this.Next = NextNode;       
                NextNode.Parent = Parent;
                
                Parent.Nodes.Insert(Parent.Nodes.IndexOf(this)+1,NextNode);
                Parent.SortKey();
                if (Parent.Keys.Count>=MaxOrder)
                {
                    return  ( Parent.Split());
                }
            }
            return Parent;
        }
    }
    class   ChildNode : Node
    {
        public List<string> Keys;
        public List<Node> Nodes;
     
     
        public ChildNode(int MaxOrder)
        {
            Keys = new List<string>();
            Nodes = new List<Node>();
            this.MaxOrder = MaxOrder;
            Parent = null;
        }
        public void SortKey()
        {
            Keys.Sort((a, b) => { return int.Parse(a).CompareTo(int.Parse(b)); });
        }  
       public void Delete(string Key,ref ChildNode rootnode)
        {
            if(!Keys.Contains(Key))
            {
                Console.WriteLine("父节点删除失败没有这个Key");
                return;
            }
            Console.WriteLine("sb");
            int amount = (int)(Math.Ceiling(((float)MaxOrder / 2)) - 1);
            Keys.Remove(Key);
           
            if (Keys.Count<amount)
            {
                ChildNode Next = null;
                ChildNode Last = null;
                if (Parent == null)
                {
                    for (int i = 0; i < Nodes.Count - 1; i++)
                    {
                        (Nodes[i] as LeafNode).Records.AddRange((Nodes[i + 1] as LeafNode).Records);
                        (Nodes[i] as LeafNode).Next = null;
                        (Nodes[i] as LeafNode).Parent = null;
                    }
                    rootnode = null;
                    return;
                }

                if (Parent.Nodes.FindIndex((a) => { return a == this; }) + 1<=Parent.Nodes.Count-1)
                Next = Parent.Nodes[Parent.Nodes.FindIndex((a) => { return a == this; }) + 1] as ChildNode;
                 
                if (Parent.Nodes.FindIndex((a) => { return a == this; }) - 1>=0)
                Last = Parent.Nodes[Parent.Nodes.FindIndex((a) => { return a == this; }) - 1] as ChildNode;

               
                if(Last!=null)
                {
                    if(Last.Keys.Count>amount)
                    {
                        Keys.Add(Last.Keys[Last.Keys.Count - 1]);
                        Nodes.Add(Last.Nodes[Last.Nodes.Count - 1]);
                        Last.Nodes.RemoveAt(Last.Nodes.Count - 1);
                        Last.Keys.RemoveAt(Last.Keys.Count - 1);
                        SortKey();

                        return;
                    }
                }

                if (Next != null)
                {
                    if (Next.Keys.Count > amount)
                    {
                        Keys.Add(Next.Keys[0]);
                        Next.Keys.RemoveAt(0);
                        Nodes.Add(Next.Nodes[0]);
                        Next.Nodes.RemoveAt(0);
                        SortKey();

                        return;
                    }
                    
                }

               
                if(Next!=null&&Last==null)
                {
                    Keys.AddRange(Parent.Keys);
                    Keys.AddRange(Next.Keys);
                    Nodes.AddRange(Next.Nodes);
                    Parent = Parent.Parent;
                    if(Parent.Parent!=null)
                    Parent.Parent.Nodes[Parent.Parent.Nodes.FindIndex((a)=> { return a == Parent; })]=this;
                    SortKey();
                    return;
                }

                if (Next == null && Last != null)
                {
                    Last.Keys.AddRange(Parent.Keys);
                    Last.Keys.AddRange(Keys);
                    Last.Nodes.AddRange(Nodes);
                    Last.Parent = Parent.Parent;
                    if (Last.Parent.Parent != null)
                        Last.Parent.Parent.Nodes[Last.Parent.Parent.Nodes.FindIndex((a) => { return a == Last.Parent; })] = this;
                    SortKey();
                    return;
                }


            }
        }
        public ChildNode Split()
        {
            if(Parent==null)
            {
                ChildNode node = new ChildNode(MaxOrder);
                ChildNode NextNode = new ChildNode(MaxOrder);
                node.Keys.Add(Keys[MaxOrder / 2]);
             
                for (int i = MaxOrder / 2+1; i < MaxOrder; i++)
                {
               
                    NextNode.Keys.Add(Keys[i]);
                   
                }
              
                Keys.RemoveRange(MaxOrder / 2 + 1, MaxOrder - (MaxOrder / 2 + 1));
                Keys.Remove(Keys[MaxOrder / 2]);

                for (int i = Nodes.Count / 2 ; i < Nodes.Count; i++)
                {
                    Nodes[i].Parent = NextNode;
                    NextNode.Nodes.Add(Nodes[i]);
                     
                }
                for (int i = Nodes.Count / 2; i < Nodes.Count; i++)
                {
                    Nodes.Remove(Nodes[i]);
                }
  
                this.Parent = node;
                NextNode.Parent = node;
                node.Nodes.Add(this);
                node.Nodes.Add(NextNode);
                Parent.SortKey();
                
            }
            else
            {
                Parent.Keys.Add(Keys[MaxOrder / 2]);
                ChildNode NextNode = new ChildNode(MaxOrder);
                for (int i = MaxOrder / 2+1; i < MaxOrder; i++)
                {
               
                    NextNode.Keys.Add(Keys[i]);
                   
                }
                Keys.RemoveRange(MaxOrder / 2 + 1, MaxOrder - (MaxOrder / 2 + 1));
                Keys.Remove(Keys[MaxOrder / 2]);
                for (int i = Nodes.Count / 2; i < Nodes.Count; i++)
                {
                    Nodes[i].Parent = NextNode;
                    NextNode.Nodes.Add(Nodes[i]);
                  
                }
                for (int i = Nodes.Count / 2; i < Nodes.Count; i++)
                {
                    Nodes.Remove(Nodes[i]);
                }
                NextNode.Parent = Parent;
                Parent.Nodes.Insert(Parent.Nodes.IndexOf(this)+1,NextNode);
                Parent.SortKey();
                if (Parent.Keys.Count >= MaxOrder)
                {
                    return ( Parent.Split());
                }
            }
            return Parent;
        }
    }
    class B_Tree
    {
        public int MaxOrder;
        public ChildNode RootNode;
        public LeafNode RootLeafNode;
        public B_Tree(int Order)
        {
            MaxOrder = Order;
            RootLeafNode = null;
            RootNode = null;
        }


        public void ShowRoot()
        {
            ChildNode cn = RootNode;
            if (cn != null)
            {
                for (int i = 0; i < cn.Keys.Count; i++)
                {
                    Console.Write(cn.Keys[i] + " ");
                }
            }

        }
        public void Show()
        {
            LeafNode ln = RootLeafNode;
           
            while (ln != null)
            {
                for (int i = 0; i < ln.Records.Count; i++)
                {
                    Console.Write(ln.Records[i].Key+" ");
                }
                Console.Write("|||||");     
                ln = ln.Next;
            }
        }
        public void Delete(string key,ref ChildNode rootnode)
        {
            LeafNode temp;
            if(!Find(key,out temp))
            {
                Console.WriteLine("删除失败!没有这个Key");
                return;
            }
            temp.DeleteRecord(key,ref rootnode);
            Console.WriteLine("删除成功!");


        }
        public bool Find(string key, out Record r)  //查找并返回查找到的叶子节点
        {
            Node node = RootNode;
            r = null;
            if (node == null)
            {
                if (RootLeafNode == null)
                    return false;
            }
            while (node != null)
            {
                bool Insert = false;
                if (node is ChildNode)
                {
                    for (int i = 0; i < (node as ChildNode).Keys.Count; i++)
                    {
                        if (int.Parse(key) < int.Parse((node as ChildNode).Keys[i]))
                        {

                            Insert = true;

                            node = (node as ChildNode).Nodes[i];
                            break;
                        }
                    }
                    if (!Insert)
                    {
                        node = (node as ChildNode).Nodes[(node as ChildNode).Keys.Count];
                    }

                }
                else
                {

                    for (int i = 0; i < (node as LeafNode).Records.Count; i++)
                    {

                        if (int.Parse(key) == int.Parse((node as LeafNode).Records[i].Key))
                        {
                            r = (node as LeafNode).Records[i];
                            Console.WriteLine("找到了");
                            return true;
                        }
                    }
                    return false;
                }

            }

            for (int i = 0; i < RootLeafNode.Records.Count; i++)
            {
                if (int.Parse(key) == int.Parse(RootLeafNode.Records[i].Key))
                {
                    r = RootLeafNode.Records[i];
                    Console.WriteLine("找到了");
                    return true;
                }
            }

            return false;


        }
        public bool Find(string key,out LeafNode ln)  //查找并返回查找到的叶子节点
        {
            Node node = RootNode;
            ln = null;
            if(node==null)
            {
                if (RootLeafNode == null)
                    return false;
            }
           while(node!=null)
            {        
                bool Insert = false;
                if (node is ChildNode)
                {
                    for (int i = 0; i < (node as ChildNode).Keys.Count; i++)
                    {
                        if (int.Parse(key) < int.Parse((node as ChildNode).Keys[i]))
                        {
                       
                            Insert = true;
                           
                            node = (node as ChildNode).Nodes[i]; 
                            break;
                        }
                    }
                    if (!Insert)
                    {
                        node = (node as ChildNode).Nodes[(node as ChildNode).Keys.Count];
                    }
                   
                }
                else
                {
                 
                    for (int i = 0; i < (node as LeafNode).Records.Count; i++)
                    {
                     
                        if(int.Parse(key)==int.Parse((node as LeafNode).Records[i].Key))
                        {
                            ln = (node as LeafNode);
                            Console.WriteLine("找到了");
                            return true;
                        }
                    }
                    return false;
                }
               
            }
           
            for (int i = 0; i < RootLeafNode.Records.Count; i++)
            {
                if (int.Parse(key) == int.Parse(RootLeafNode.Records[i].Key))
                {
                    ln = RootLeafNode;
                    Console.WriteLine("找到了");
                    return true;
                }
            }

            return false;


        }
        public bool Find(string key)  //查找
        {
            Node node = RootNode;
          
            while (node != null)
            {
                bool Insert = false;
                if (node is ChildNode)
                {
                    for (int i = 0; i < (node as ChildNode).Keys.Count; i++)
                    {
                        if (int.Parse(key) <int.Parse((node as ChildNode).Keys[i]))
                        {

                            Insert = true;
                            node = (node as ChildNode).Nodes[i];
                            break;
                        }
                    }
                    if (!Insert)
                    {
                        node = (node as ChildNode).Nodes[(node as ChildNode).Keys.Count];
                    }

                }
                else
                {

                    for (int i = 0; i < (node as LeafNode).Records.Count; i++)
                    {

                        if (int.Parse(key) == int.Parse((node as LeafNode).Records[i].Key))
                        {
                        
                            Console.WriteLine("找到了");
                            return true;
                        }
                    }
                    return false;
                }

            }

            for (int i = 0; i < RootLeafNode.Records.Count; i++)
            {
                if (int.Parse(key) == int.Parse(RootLeafNode.Records[i].Key))
                {       
                    Console.WriteLine("找到了");
                    return true;
                }
            }

            return false;


        }
        public void Insert(Record record)
        {
            if(RootNode==null)
            {
                if (RootLeafNode == null)
                {
                   
                    LeafNode node = new LeafNode(MaxOrder);
                    
                    RootLeafNode = node;
                    RootLeafNode.InsertRecord(record,ref RootNode);
                }
                else
                {
                    RootLeafNode.InsertRecord(record,ref RootNode);
                    
                }
            }
            else
            {
                LeafNode ln = RootLeafNode;
                while(ln.Next!=null)
                {  
                    if(int.Parse( record.Key)<=int.Parse( ln.Records[ln.Records.Count-1].Key))
                    {               
                        break;
                    }
                    ln = ln.Next;
                }
                 ln.InsertRecord(record,ref RootNode);
            }
        }
       
    }
    
}
