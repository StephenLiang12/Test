using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture
{
    public class LinkedListNode
    {
        public int val;
        public LinkedListNode next;
    };

    [TestClass]
    public class CodeTestFixture
    {
        [TestMethod]
        public void AbleToNumber()
        {
            string s1 = "A";
            Console.WriteLine(StringToNumber(s1));
            string s2 = "AA";
            Console.WriteLine(StringToNumber(s2));
        }

        public int StringToNumber(string s)
        {
            int n = 0;
            for (int i = 0; i < s.Length; i++)
            {
                n = n * 26 + (s[i] - 64);
            }
            return n;
        }

        [TestMethod]
        public void CheckIsBalanced()
        {
            string s1 = "([52]12)";
            Console.WriteLine(isBalanced(s1));
            string s2 = "40[1223[45(12))]";
            Console.WriteLine(isBalanced(s2));
            string s3 = "135689103";
            Console.WriteLine(isBalanced(s3));
            string s4 = null;
            Console.WriteLine(isBalanced(s4));
            string s5 = string.Empty;
            Console.WriteLine(isBalanced(s5));
        }

        public bool isBalanced(string input)
        {
            if (input == null)
                return false;
            Stack<char> stack = new Stack<char>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] >='0' && input[i] <= '9')
                    continue;
                if (input[i] == '(' || input[i] == '[')
                    stack.Push(input[i]);
                else if (input[i] == ')' || input[i] == ']')
                {
                    if (stack.Count > 0)
                    {
                        char top = stack.Pop();
                        if ((top == '(' && input[i] == ')') ||
                            (top == '[' && input[i] == ']'))
                            continue;
                    }
                        return false;
                }
                else
                {
                    return false;
                }
            }
            if (stack.Count > 0)
                return false;
            return true;
        }

        public static LinkedListNode _insert_node_into_singlylinkedlist(LinkedListNode head, int val)
        {
            if (head == null)
            {
                head = new LinkedListNode();
                head.val = val;
                head.next = null;
            }
            else
            {
                LinkedListNode end = head;
                while (end.next != null)
                {
                    end = end.next;
                }
                LinkedListNode node = new LinkedListNode();
                node.val = val;
                node.next = null;
                end.next = node;
            }
            return head;
        }

        [TestMethod]
        public void Main()
        {
            LinkedListNode root = new LinkedListNode();
            var node = root;
            node.val = 10;
            node.next = new LinkedListNode();
            node = node.next;
            node.val = 20;
            node.next = new LinkedListNode();
            node = node.next;
            node.val = 30;
            node.next = new LinkedListNode();
            node = node.next;
            node.val = 40;
            node.next = new LinkedListNode();
            node = node.next;
            node.val = 50;
            var newRoot = manipulateList(root);
            node = newRoot;
            while (node != null)
            {
                Console.WriteLine(node.val);
                node = node.next;
            }
        }

        static LinkedListNode manipulateList(LinkedListNode root)
        {
            if (root == null)
                return null;
            var middle = GetHalfLinkedLinkNode(root);
            return combineLinkedList(root, middle);
        }

        private static LinkedListNode GetHalfLinkedLinkNode(LinkedListNode root)
        {
            var node = root;
            var middle = root;
            while (node != null)
            {
                node = node.next;
                if (node == null)
                    return middle;
                node = node.next;
                middle = middle.next;
            }
            return middle;
        }

        private static LinkedListNode combineLinkedList(LinkedListNode root1, LinkedListNode root2)
        {
            var start = new LinkedListNode();
            var currentNode = start;
            currentNode.val = root1.val;
            var node1 = root1.next;
            var node2 = root2;
            while (node1 != root2)
            {
                currentNode.next = new LinkedListNode();
                currentNode.next.val = node2.val;
                currentNode = currentNode.next;
                node2 = node2.next;
                currentNode.next = new LinkedListNode();
                currentNode.next.val = node1.val;
                currentNode = currentNode.next;
                node1 = node1.next;
            }
            while (node2 != null)
            {
                currentNode.next = new LinkedListNode();
                currentNode.next.val = node2.val;
                node2 = node2.next;
                currentNode = currentNode.next;
            }
            return start;
        }
    }
}