using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using EmrConstant;

namespace EMR
{
    class EmrDocuments
    {
        private  EmrDocument[] emrDocuments = new EmrDocument[EmrConstant.IntGeneral.DocumentMax];
        private bool[] inUse = new bool[EmrConstant.IntGeneral.DocumentMax];
        private bool[] isNew = new bool[EmrConstant.IntGeneral.DocumentMax];
        /*  permissionLevel is the Operator's permission on the emrDocuments and set when opening */
        private EmrConstant.PermissionLevel[] permissionLevel =
            new EmrConstant.PermissionLevel[EmrConstant.IntGeneral.DocumentMax];

        public EmrDocuments()
        {
            for (int i = 0; i < EmrConstant.IntGeneral.DocumentMax; i++ )
            {
                inUse[i] = false;
                isNew[i] = true;
                permissionLevel[i] = EmrConstant.PermissionLevel.ReadWrite;
            }
        }
        /* Find an index(subscript) of which the element is not in use .*/
        private int AvailableIndex()
        {
            for(int i=0; i<EmrConstant.IntGeneral.DocumentMax; i++)
            {
                if (inUse[i] == false) return i;
            }
            return -1;
        }
       
        /* -----------------------------------------------------------------------------------
         * Create new an EmrDocument.
         * Parameter:
         *      registryID -- The registry identifier of a patient for a visit to the hospital.
         *      permission -- The current operator has what privilege on this EmrDocument.
         * Attributes:
         *      inUse directs that the array element is in use.
         *      inNew directs that the document has not been created and is empty.
         * Return:
         *      index -- Direct to the new EmrDocument if successful.
         *      -1 -- No new EmrDocument is created.
         ------------------------------------------------------------------------------------- */
        public int open(string registryID, PermissionLevel permission, string archiveNum)
        {
            int index = AvailableIndex();
            if (index < 0) return -1;
            
            if (emrDocuments[index] == null) emrDocuments[index] = new EmrDocument();
            int errno = emrDocuments[index].Create(registryID, archiveNum);
            if (errno == 0 || errno == -3)
            {
                inUse[index] = true;
                permissionLevel[index] = permission;
                if (errno == -3) permissionLevel[index] = PermissionLevel.ReadOnly;
                if (emrDocuments[index].Get().DocumentElement.Attributes[AttributeNames.Series].Value == "0")
                    isNew[index] = true;
                else
                    isNew[index] = false;
                return index;
            }
            return errno;          
        }
        public void SetOld(int index)
        {
            isNew[index] = false;
        }
        public Boolean IsNew(int index)
        {
            if (isNew[index]) return true;
            else return false;
        }
        /* ------------------------------------------------------------------------------------
         * Destroy the specified EmrDocument and release it's resource
         * Parameter:
         *      index -- Direct to the EmrDocument to be removed..
         --------------------------------------------------------------------------------------*/
        //public void close(int index)
        //{
        //    if (index < 0 || index >= EmrConstant.IntGeneral.DocumentMax) return;
        //    string registryID = 
        //        emrDocuments[index].Get().DocumentElement.Attributes[AttributeNames.RegistryID].Value;

        //    emrDocuments[index].Destroy(registryID, permissionLevel[index]);
        //    inUse[index] = false;
        //    return;
        //}
        /* ------------------------------------------------------------------------------------
         * Destroy the all EmrDocument
         --------------------------------------------------------------------------------------*/
        //public void closeAll()
        //{
        //    for (int index = 0; index < EmrConstant.IntGeneral.DocumentMax; index++)
        //    {
        //        if (inUse[index])
        //        {
        //            string registyID = 
        //                emrDocuments[index].Get().DocumentElement.Attributes[AttributeNames.RegistryID].Value;
        //            emrDocuments[index].Destroy(registyID, permissionLevel[index]);
        //            inUse[index] = false;
        //        }
        //    }
        //}
        /* ------------------------------------------------------------------------------------
         * Return the specified XmlDocument.
         * Parameter:
         *      index -- Direct to the EmrDocument to be returned.
         * Return:
         *      XmlNode -- EmrDocument
         --------------------------------------------------------------------------------------*/
        public XmlDocument Get(int index)
        {
            if (index < 0 || index >= EmrConstant.IntGeneral.DocumentMax) return null;
            return emrDocuments[index].Get();
        }
        public void Set(EmrDocument emrd)
        {
            emrDocuments[0] = emrd;
        }
        /* ------------------------------------------------------------------------------------
         * Return the permission on the specified EmrDocument.
         * Parameter:
         *      index -- Direct to the EmrDocument to be returned.
         * Return:
         *      permissionlevel
         * --------------------------------------------------------------------------------------*/
        public EmrConstant.PermissionLevel GetPermission(int index)
        {
            if (index < 0 || index >= EmrConstant.IntGeneral.DocumentMax)
                return EmrConstant.PermissionLevel.NoPrivilege;
            return permissionLevel[index];
        }

        /* ------------------------------------------------------------------------------------
         * Return the specified EmrDocument.
         * Parameter:
         *      index -- Direct to the EmrDocument to be returned.
         * Return:
         *      XmlNode -- EmrDocument
         --------------------------------------------------------------------------------------*/
        public EmrDocument GetEmrDocument(int index)
        {
            if (index < 0 || index >= IntGeneral.DocumentMax) return null;
            return emrDocuments[index];
        }
    }
}
