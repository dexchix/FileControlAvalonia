using FileControlAvalonia.Core;
using FileControlAvalonia.SettingsApp;
using Opc;
using Opc.Da;
using System;
using System.Threading.Tasks;

namespace FileControlAvalonia.DataBase
{
    public class OpcClass
    {
        /// <summary>
        /// Строка подключения к серверу ОРС
        /// </summary>
        static URL Url { get; set; }
        /// <summary>
        /// Объект - сервер ОРС
        /// </summary>
        static Opc.Da.Server Server { get; set; }
        /// <summary>
        /// Группа для чтения
        /// </summary>
        static Subscription GroupWrite { get; set; }
        /// <summary>
        /// Подписка
        /// </summary>
        static SubscriptionState GroupState { get; set; }

        static Item[] Items { get; set; } = new Item[1];

        /// <summary>
        /// Подключение к ОРС-серверу
        /// </summary>
        /// <returns></returns>
        public static bool Connect()
        {
            try
            {
                Url = new URL(SettingsManager.AppSettings.OpcConnectionString);
                Server = new Opc.Da.Server(new OpcCom.Factory(), Url);
                Server.Connect(Url, new ConnectData(new System.Net.NetworkCredential()));
                GroupState = new SubscriptionState()
                {
                    Name = "Group",
                    UpdateRate = 1000, /* this isthe time between every reads from OPC server*/
                    Active = false /*this must be true if you the group has to read value*/
                };
                GroupWrite = (Subscription)Server.CreateSubscription(GroupState);
                return true;
            }
            catch (Exception ex)
            {
                Logger.logger.Error("Ошибка подключения к ОРС-серверу: {0}", ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Асинхронная запись в ОРС-сервер
        /// </summary>
        public async static Task WriteToOpcAsync(int state,Comprasion compare)
        {
            try
            {
                if (await Task.Factory.StartNew(() => Connect()))
                {
                    if (SettingsManager.AppSettings.OpcCommonTag != "")
                    {
                        Write(SettingsManager.AppSettings.OpcCommonTag, state);
                    }
                    if (SettingsManager.AppSettings.OpcCountTag != "")
                    {
                        Write(SettingsManager.AppSettings.OpcCountTag, compare.TotalFiles);
                    }
                    if (SettingsManager.AppSettings.OpcFailedTag != "")
                    {
                        Write(SettingsManager.AppSettings.OpcFailedTag, compare.FailedChecked);
                    }
                    if (SettingsManager.AppSettings.OpcPassedTag != "")
                    {
                        Write(SettingsManager.AppSettings.OpcPassedTag, compare.Checked);
                    }
                    if (SettingsManager.AppSettings.OpcNoAccessTag != "")
                    {
                        Write(SettingsManager.AppSettings.OpcNoAccessTag, compare.NoAccess);
                    }
                    if (SettingsManager.AppSettings.OpcNotFoundTag != "")
                    {
                        Write(SettingsManager.AppSettings.OpcNotFoundTag, compare.NotFound);
                    }
                    if (SettingsManager.AppSettings.OpcSemiPassedTag != "")
                    {
                        Write(SettingsManager.AppSettings.OpcSemiPassedTag, compare.PartialChecked);
                    }
                    Server.Disconnect();
                }
            }
            catch(Exception ex)
            {
                Logger.logger.Error("Ошибка записи в ОРС-серверу: {0}", ex.Message);
            }
           
        }
       
        /// <summary>
        /// Запись в OPC-тег
        /// </summary>
        /// <returns></returns>
        public static bool Write(string tag, object value)
        {
            try
            {
                Items = GroupWrite.AddItems(new Item[]
                {
                    new Item() { ItemName = tag }
                });
                ItemValue[] writeValues = new ItemValue[1] { new ItemValue() { ServerHandle = Items[0].ServerHandle, Value = value } };
                GroupWrite.Write(writeValues);
                GroupWrite.RemoveItems(Items);
                return true;
            }
            catch (Exception ex)
            {
                Logger.logger.Error("Ошибка записи значения в тег {0}: {1}", tag, ex.Message);
                return false;
            }
        }
    }
}
