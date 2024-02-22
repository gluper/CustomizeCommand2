using _3S.CoDeSys.Core;
using _3S.CoDeSys.Core.Commands;
using _3S.CoDeSys.Core.Components;
using _3S.CoDeSys.Core.Objects;
using _3S.CoDeSys.Core.Online;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomizeCommand
{
    [TypeGuid("{AB009734-D9C0-4AFF-B16E-0E3607626CC1}")]
    public class MyCreateBootApplicationCommand : ICustomizedStandardCommand
    {
        private readonly Guid CREATE_BOOTAPPLICATION_COMMAND_GUID = new Guid("{cf46872e-7cf6-484e-acee-f8b9da763449}");
        public Guid OriginalCommandGuid => CREATE_BOOTAPPLICATION_COMMAND_GUID;

        public void AddedToUI(ICommand originalCommand)
        {
            originalCommand.AddedToUI();
        }

        public string[] CreateBatchArguments(bool bInvokedByContextMenu, IStandardCommand originalCommand)
        {
            return originalCommand.CreateBatchArguments();
        }

        public void ExecuteBatch(string[] arguments, ICommand originalCommand)
        {
            // perform original command
            originalCommand.ExecuteBatch(arguments);

            // additional functions here
            DownloadFile();
        }


        public string[] GetBatchCommand(ICommand originalCommand)
        {
            return originalCommand.BatchCommand;
        }

        public Guid GetCategory(ICommand originalCommand)
        {
            return originalCommand.Category;
        }

        public string GetDescription(ICommand originalCommand)
        {
            return originalCommand.Description;
        }

        public bool GetEnabled(ICommand originalCommand)
        {
            return originalCommand.Enabled;
        }

        public Icon GetLargeIcon(ICommand originalCommand)
        {
            return GetSmallIcon(originalCommand);
        }

        public string GetName(ICommand originalCommand)
        {
            return originalCommand.Name + " CUSTOMIZED! ";
        }

        public Icon GetSmallIcon(ICommand originalCommand)
        {
            return Res.HelloIcon;
        }

        public string GetToolTipText(ICommand originalCommand)
        {
            return originalCommand.ToolTipText;
        }

        public bool IsVisible(bool bContextMenu, ICommand originalCommand)
        {
            return originalCommand.IsVisible(bContextMenu);
        }

        public void RemovedFromUI(ICommand originalCommand)
        {
            originalCommand.RemovedFromUI();
        }

        private void DownloadFile()
        {
            IOnlineDevice3 onlineDevice = GetOnlineDeviceForApplication() as IOnlineDevice3;
            if (onlineDevice == null)
                return;


            object connectObject = null;

            connectObject = new object();
            try
            {
                connectObject = new object();

                onlineDevice.SharedConnect(connectObject);

                // Execute the various online services
                string stPath = @"D:\temp\testFile.txt";
                Stream downloadStream = new FileStream(stPath, FileMode.Open, FileAccess.Read); 
                IFileDownload fileDownload = onlineDevice.CreateFileDownload(downloadStream, "testFileInPLC.txt");

                IAsyncResult asyncResult = fileDownload.BeginDownload(true, null, null);

                while (!asyncResult.IsCompleted)
                {
                    Application.DoEvents();
                }
                downloadStream.Close();
                fileDownload.EndDownload(asyncResult);
            }
            catch
            {
                // Do your optional error reporting here
            }
            finally
            {
                if (onlineDevice.IsConnected)
                {
                    try
                    {
                        onlineDevice.SharedDisconnect(connectObject);
                    }
                    catch { } // Fail silently, we are already disconnected
                }
            }
        }

        private IOnlineDevice GetOnlineDeviceForApplication()
        {
            IOnlineDevice onlineDevice = null;
            if (APEnvironment.Engine.Projects.PrimaryProject != null)
            {
                Guid guidActiveApp = APEnvironment.Engine.Projects.PrimaryProject.ActiveApplication;
                int nHandle = APEnvironment.Engine.Projects.PrimaryProject.Handle;
                if ( APEnvironment.ObjectMgr.ExistsObject(nHandle, guidActiveApp))
                {
                    IMetaObjectStub stub = APEnvironment.ObjectMgr.GetMetaObjectStub(nHandle, guidActiveApp);
                    while (stub.ParentObjectGuid != Guid.Empty)
                        stub = APEnvironment.ObjectMgr.GetMetaObjectStub(nHandle, stub.ParentObjectGuid);
                    Guid guidDeviceObject = stub.ObjectGuid;
                    onlineDevice = APEnvironment.OnlineMgr.GetOnlineDevice(guidDeviceObject);
                }
            }
            return onlineDevice;
        }
    }

}
