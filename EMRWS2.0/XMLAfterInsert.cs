using System;
using System.Collections.Generic;
using System.Text;
using Word = Microsoft.Office.Interop.Word;

namespace EMR
{
   public  class XMLAfterInsert
    {
       public static void deleteXMLMark(Microsoft.Office.Interop.Word.XMLNode NewXMLNode)
        {
            try
            {
                Word.XMLNodes nodes = NewXMLNode.ChildNodes;
                if (nodes != null && nodes.Count > 0)
                {
                    for (int i = 1; i <= nodes.Count; i++)
                    {
                        deleteXMLMark(nodes[i]);
                    }
                }
                NewXMLNode.Delete();

                //Word.XMLNodes nodes = NewXMLNode.ChildNodes;
                
                //if (nodes != null && nodes.Count > 0)
                //{
                //    for (int i = 1; i < nodes.Count; i++)
                //    {
                //        //deleteXMLMark(nodes[i]);
                //    }
                //}
                //Word.Range r = NewXMLNode.Range;
                //int iStart = r.Start - 1;
                //r.Start = iStart;
                //r.End = iStart;
                //r.Delete();
            }
            catch (Exception ex)
            {

            }
        }
      public static  void myDoc_XMLAfterInsert(Word.XMLNode NewXMLNode, bool InUndoRedo)
        {
            if (InUndoRedo)
            {
                return;
            }
            try
            {
                object obj = NewXMLNode.NodeValue;
            }
            catch (Exception)
            {
                return;
            }
            deleteXMLMark(NewXMLNode);
            return;
        }
    }
}
