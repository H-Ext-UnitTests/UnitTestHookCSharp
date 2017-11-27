// API requirement to function

#define EXT_IUTIL
#define EXT_ICINIFILE
#define EXT_ICOMMAND

//Future API support

//#define EXT_HKEXTERNAL            // TBD

#if DO_NOT_INCLUDE_THIS
addon_info EXTPluginInfo = { "UnitTest Hook C Sharp", "1.0.0.0",
                            "DZS|All-In-One, founder of DZS",
                            "Used for verifying each hook API are working right in C# language under C99 standard.",
                            "UnitTestHook",
                            "unit_test",
                            "test_unit",
                            "unit test",
                            "[unit]test",
                            "test[unit]"};
#endif

/*
 * This link is for effective usage in unmanaged code (not for C# code) to load managed dll.
 * http://stackoverflow.com/questions/773476/how-to-split-dot-net-hosting-function-when-calling-via-c-dll
 */

using System;
using System.Text;

using RGiesecke.DllExport;
using System.Runtime.InteropServices;

namespace UnitTestHookCSharp {
    public class Addon {
        public static uint EAOhashID;
        public static string iniFileStr = "HookTestCSharp.ini";
        public static Addon_API.ICIniFileClass pICIniFile;
        public static Addon_API.IUtil pIUtil;
        public static Addon_API.ICommand pICommand;

        [DllExport("EXTOnEAOLoad", CallingConvention = CallingConvention.Cdecl)]
        public static Addon_API.EAO_RETURN EXTOnEAOLoad(uint uniqueHash) {
            EAOhashID = uniqueHash;
            pICIniFile = Addon_API.Interface.getICIniFile(EAOhashID);
            pIUtil = Addon_API.Interface.getIUtil(EAOhashID);
            pICommand = Addon_API.Interface.getICommand(EAOhashID);

            if (!pICIniFile.isNotNull()) {
                goto initFail;
            }
            if (pICIniFile.m_open_file(iniFileStr)) {
                if (!pICIniFile.m_delete_file(iniFileStr)) {
                    goto initFail;
                }
                if (pICIniFile.m_open_file(iniFileStr)) {
                    goto initFail;
                }
            }
            if (!pICIniFile.m_create_file(iniFileStr)) {
                goto initFail;
            }
            if (!pICIniFile.m_open_file(iniFileStr)) {
                goto initFail;
            }

            // This is needed in order to preserve function pointer address
            eao_unittesthook_savePtr = eao_unittesthook_save;
            GC.KeepAlive(eao_unittesthook_savePtr);

            if (!pICommand.m_add(EAOhashID, eao_unittesthook_saveStr, eao_unittesthook_savePtr, "unit_test", 1, 1, false, HEXT.modeAll)) {
                goto initFail;
            }

            return Addon_API.EAO_RETURN.OVERRIDE;
            initFail:
            if (pICIniFile.isNotNull()) {
                pICIniFile.m_release();
            }
            if (pICommand.isNotNull()) {
                pICommand.m_delete(EAOhashID, eao_unittesthook_savePtr, eao_unittesthook_saveStr);
            }
            GC.Collect();
            return Addon_API.EAO_RETURN.FAIL;
        }

        [DllExport("EXTOnEAOUnload", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnEAOUnload() {
            if (pICIniFile.isNotNull()) {
                pICIniFile.m_save();
                pICIniFile.m_close();
                pICIniFile.m_release();
            }
            if (pICommand.isNotNull()) {
                pICommand.m_delete(EAOhashID, eao_unittesthook_savePtr, eao_unittesthook_saveStr);
            }
            GC.Collect();
        }
        public static string eao_unittesthook_saveStr = "eao_unittesthook_save_csharp";
        //This is needed in order to preserve function pointer address
        public static Addon_API.CmdFunc eao_unittesthook_savePtr;
        public static Addon_API.CMD_RETURN eao_unittesthook_save([In] Addon_API.PlayerInfo plI, [In, Out] ref Addon_API.ArgContainerVars arg, [In] Addon_API.MSG_PROTOCOL protocolMsg, [In] uint idTimer, [In, Out] boolOption showChat) {
            if (pICIniFile.isNotNull()) {
                pICIniFile.m_save();
            }
            return Addon_API.CMD_RETURN.SUCC;
        }

        public static string[] HookNames = { "EXTOnPlayerJoin"
                            , "EXTOnPlayerQuit"
                            , "EXTOnPlayerDeath"
                            , "EXTOnPlayerChangeTeamAttempt"
                            , "EXTOnPlayerJoinDefault"
                            , "EXTOnNewGame"
                            , "EXTOnEndGame"
                            , "EXTOnObjectInteraction"
                            , "EXTOnWeaponAssignmentDefault"
                            , "EXTOnWeaponAssignmentCustom"
                            , "EXTOnServerIdle"
                            , "EXTOnPlayerScoreCTF"
                            , "EXTOnWeaponDropAttemptMustBeReadied"
                            , "EXTOnPlayerSpawn"
                            , "EXTOnPlayerVehicleEntry"
                            , "EXTOnPlayerVehicleEject"
                            , "EXTOnPlayerSpawnColor"
                            , "EXTOnPlayerValidateConnect"
                            , "EXTOnWeaponReload"
                            , "EXTOnObjectCreate"
                            , "EXTOnKillMultiplier"
                            , "EXTOnVehicleRespawnProcess"
                            , "EXTOnObjectDeleteAttempt"
                            , "EXTOnObjectDamageLookupProcess"
                            , "EXTOnObjectDamageApplyProcess"
                            // Featured in 0.5.1 and newer
                            , "EXTOnMapLoad"
                            , "EXTOnAIVehicleEntry"
                            , "EXTOnWeaponDropCurrent"
                            , "EXTOnServerStatus"
                            // Featured in 0.5.2.3 and newer
                            , "EXTOnPlayerUpdate"
                            , "EXTOnMapReset"
                            // Featured in 0.5.3.0 and newer
                            , "EXTOnObjectCreateAttempt"
                            // Featured in 0.5.3.2 and newer
                            , "EXTOnGameSpyValidationCheck"
                            // Featured in 0.5.3.3 and newer
                            , "EXTOnWeaponExchangeAttempt"
                            // Featured in 0.5.3.4 and newer
                            , "EXTOnObjectDelete" };

        public struct checkList {
            public int EXTOnPlayerJoin;
            public int EXTOnPlayerQuit;
            public int EXTOnPlayerDeath;
            public int EXTOnPlayerChangeTeamAttempt;
            public int EXTOnPlayerJoinDefault;
            public int EXTOnNewGame;
            public int EXTOnEndGame;
            public int EXTOnObjectInteraction;
            public int EXTOnWeaponAssignmentDefault;
            public int EXTOnWeaponAssignmentCustom;
            public int EXTOnServerIdle;
            public int EXTOnPlayerScoreCTF;
            public int EXTOnWeaponDropAttemptMustBeReadied;
            public int EXTOnPlayerSpawn;
            public int EXTOnPlayerVehicleEntry;
            public int EXTOnPlayerVehicleEject;
            public int EXTOnPlayerSpawnColor;
            public int EXTOnPlayerValidateConnect;
            public int EXTOnWeaponReload;
            public int EXTOnObjectCreate;
            public int EXTOnKillMultiplier;
            public int EXTOnVehicleRespawnProcess;
            public int EXTOnObjectDeleteAttempt;
            public int EXTOnObjectDamageLookupProcess;
            public int EXTOnObjectDamageApplyProcess;
            public int EXTOnMapLoad;
            public int EXTOnAIVehicleEntry;
            public int EXTOnWeaponDropCurrent;
            public int EXTOnServerStatus;
            public int EXTOnPlayerUpdate;
            public int EXTOnMapReset;
            public int EXTOnObjectCreateAttempt;
            public int EXTOnGameSpyValidationCheck;
            public int EXTOnWeaponExchangeAttempt;
            public int EXTOnObjectDelete;
        };
        public static checkList checkHooks = new checkList();
        public const int MAX_HOOK_COUNTER = 5;
        public const string nullStr = "NULL";

        // List of all available hooks

        [DllExport("EXTOnPlayerJoin", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnPlayerJoin(Addon_API.PlayerInfo plI) {
            if (checkHooks.EXTOnPlayerJoin < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerJoin++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                vars[0] = plI.plS.Name;
                vars[1] = plI.plEx.adminLvl;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, Admin: {1:hd}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[0], checkHooks.EXTOnPlayerJoin.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnPlayerQuit", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnPlayerQuit(Addon_API.PlayerInfo plI) {
            if (checkHooks.EXTOnPlayerQuit < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerQuit++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                vars[0] = plI.plS.Name;
                vars[1] = plI.plEx.adminLvl;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, Admin: {1:hd}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[1], checkHooks.EXTOnPlayerQuit.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnPlayerDeath", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnPlayerDeath([In] Addon_API.PlayerInfo killer, [In] Addon_API.PlayerInfo victim, [In] int mode, [In, Out] boolOption show_message) {
            if (checkHooks.EXTOnPlayerDeath < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerDeath++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[4];
                vars[0] = killer.cplS != IntPtr.Zero ? killer.plS.Name : nullStr;
                vars[1] = victim.cplS != IntPtr.Zero ? victim.plS.Name : nullStr;
                vars[2] = mode;
                vars[3] = show_message.boolean;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Killer: {0:s}, Victim: {1:s}, Mode: {2:d}, showMessage: {3:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[2], checkHooks.EXTOnPlayerDeath.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnPlayerChangeTeamAttempt", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnPlayerChangeTeamAttempt([In] Addon_API.PlayerInfo plI, [In] e_color_team_index team, [In, MarshalAs(UnmanagedType.I1)] bool forceChange) {
            if (checkHooks.EXTOnPlayerChangeTeamAttempt < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerChangeTeamAttempt++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[3];
                vars[0] = plI.plS.Name;
                vars[1] = team;
                vars[2] = forceChange;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, Team: {1:d}, forceChange: {2:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[3], checkHooks.EXTOnPlayerChangeTeamAttempt.ToString(), output.ToString());
            }
            return true; //If set to false, it will prevent change team. Unless forceChange is true, this value is ignored.
        }

        [DllExport("EXTOnPlayerJoinDefault", CallingConvention = CallingConvention.Cdecl)]
        public static e_color_team_index EXTOnPlayerJoinDefault([In] s_machine_slot_ptr mS, [In] e_color_team_index cur_team) {
            if (checkHooks.EXTOnPlayerJoinDefault < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerJoinDefault++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[3];
                s_machine_slot_managed mS_managed = new s_machine_slot_managed(mS);
                vars[0] = mS_managed.data.machineIndex;
                vars[1] = mS_managed.data.SessionKey;
                vars[2] = mS_managed.data.isAvailable;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "mIndex: {0:hd}, SessionKey: {1:s}, isAvailable: {2:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[4], checkHooks.EXTOnPlayerJoinDefault.ToString(), output.ToString());
            }
            return e_color_team_index.TEAM_NONE; //If set to 0 will force set to red team. Or set to 1 will force set to blue team. -1 is default team.
        }

        [Obsolete("Do not use EXTOnNewGame hook, use EXTOnMapLoad hook instead.")]
        [DllExport("EXTOnNewGame", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnNewGame([In, MarshalAs(UnmanagedType.LPWStr)] string map) {
            if (checkHooks.EXTOnNewGame < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnNewGame++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[1];
                vars[0] = map;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Map: {0:s}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[5], checkHooks.EXTOnNewGame.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnEndGame", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnEndGame([In] int mode) {
            if (checkHooks.EXTOnEndGame < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnEndGame++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[1];
                vars[0] = mode;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Mode: {0:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[6], checkHooks.EXTOnEndGame.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnObjectInteraction", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnObjectInteraction([In] Addon_API.PlayerInfo plI, [In] s_ident obj_id, [In] s_object_ptr objectStruct, [In] Addon_API.hTagHeaderPtr hTag) {
            if (checkHooks.EXTOnObjectInteraction < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnObjectInteraction++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[5];
                s_object_managed objectStructManaged = new s_object_managed(objectStruct);
                Addon_API.hTagHeader_managed hTagManaged = new Addon_API.hTagHeader_managed(hTag);
                vars[0] = plI.plS.Name;
                vars[1] = obj_id.Tag;
                vars[2] = objectStructManaged.s_object_n.ModelTag.Tag;
                vars[3] = objectStructManaged.s_object_n.GameObject;
                vars[4] = hTagManaged.hTagHeader_n.tag_name;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, ObjectID: {1:08X}, ModelTag: {2:08X}, GameObject: {3:hd}, tagName: {4:s}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[7], checkHooks.EXTOnObjectInteraction.ToString(), output.ToString());
            }
            return true; //If set to false, it will deny interaction to an object.
        }

        [DllExport("EXTOnWeaponAssignmentDefault", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnWeaponAssignmentDefault([In] Addon_API.PlayerInfo plI, [In] s_ident owner_obj_id, [In] s_tag_reference_ptr cur_weap_id, [In] uint order, [In, Out] s_ident_ptr new_weap_id) {
            if (checkHooks.EXTOnWeaponAssignmentDefault < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnWeaponAssignmentDefault++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[5];
                s_tag_reference_managed cur_weap_id_managed = new s_tag_reference_managed(cur_weap_id);
                s_ident_managed new_weap_id_managed = new s_ident_managed(new_weap_id);
                vars[0] = plI.plS.Name;
                vars[1] = owner_obj_id.Tag;
                vars[2] = cur_weap_id_managed.data.name;
                vars[3] = order;
                vars[4] = new_weap_id_managed.data.Tag;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, OwnerObjectID: {1:08X}, Weapon Name: {2:s}, Order: {3:d}, newWeapID: {4:08X}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[8], checkHooks.EXTOnWeaponAssignmentDefault.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnWeaponAssignmentCustom", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnWeaponAssignmentCustom([In] Addon_API.PlayerInfo plI, [In] s_ident owner_obj_id, [In] s_ident cur_weap_id, [In] uint order, [In, Out] s_ident_ptr new_weap_id) {
            if (checkHooks.EXTOnWeaponAssignmentCustom < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnWeaponAssignmentCustom++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[5];
                s_ident_managed new_weap_id_managed = new s_ident_managed(new_weap_id);
                vars[0] = plI.plS.Name;
                vars[1] = owner_obj_id.Tag;
                vars[2] = cur_weap_id.Tag;
                vars[3] = order;
                vars[4] = new_weap_id_managed.data.Tag;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, OwnerObjectID: {1:08X}, curWeaponID: {2:08X}, Order: {3:d}, newWeapID: {4:08X}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[9], checkHooks.EXTOnWeaponAssignmentCustom.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnServerIdle", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnServerIdle() {
            if (checkHooks.EXTOnServerIdle < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnServerIdle++;
                pICIniFile.m_value_set(HookNames[10], checkHooks.EXTOnServerIdle.ToString(), "Idle");
            }
        }

        [DllExport("EXTOnPlayerScoreCTF", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnPlayerScoreCTF([In] Addon_API.PlayerInfo plI, [In] s_ident cur_weap_id, [In] uint team, [In, MarshalAs(UnmanagedType.I1)] bool isGameObject) {
            if (checkHooks.EXTOnPlayerScoreCTF < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerScoreCTF++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[4];
                vars[0] = plI.plS.Name;
                vars[1] = cur_weap_id.Tag;
                vars[2] = team;
                vars[3] = isGameObject;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, curWeaponID: {1:08X}, Team: {2:d}, isGameObject: {3:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[11], checkHooks.EXTOnPlayerScoreCTF.ToString(), output.ToString());
            }
            return true; //If set to false, it will prevent player to score a flag.
        }

        [DllExport("EXTOnWeaponDropAttemptMustBeReadied", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnWeaponDropAttemptMustBeReadied([In] Addon_API.PlayerInfo plI, [In] s_ident owner_obj_id, [In] s_object_ptr pl_Biped) {
            if (checkHooks.EXTOnWeaponDropAttemptMustBeReadied < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnWeaponDropAttemptMustBeReadied++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[3];
                s_biped_managed pl_Biped_managed = new s_biped_managed(pl_Biped);
                vars[0] = plI.plS.Name;
                vars[1] = owner_obj_id.Tag;
                vars[2] = pl_Biped_managed.s_object_n.sObject.ModelTag.Tag;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, ownerObjID: {1:08X}, pl_Biped: {2:08X}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[12], checkHooks.EXTOnWeaponDropAttemptMustBeReadied.ToString(), output.ToString());
            }
            return true; //If set to false, it will prevent player to drop an object.
        }

        [DllExport("EXTOnPlayerSpawn", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnPlayerSpawn([In] Addon_API.PlayerInfo plI, [In] s_ident obj_id, [In] s_object_ptr pl_Biped) {
            if (checkHooks.EXTOnPlayerSpawn < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerSpawn++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[3];
                vars[0] = plI.plS.Name;
                vars[1] = obj_id.Tag;
                vars[2] = pl_Biped.ptr;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, obj_id: {1:08X}, pl_Biped: {2:08X}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[13], checkHooks.EXTOnPlayerSpawn.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnPlayerVehicleEntry", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnPlayerVehicleEntry([In] Addon_API.PlayerInfo plI, [In, MarshalAs(UnmanagedType.I1)] bool forceEntry) {
            if (checkHooks.EXTOnPlayerVehicleEntry < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerVehicleEntry++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                vars[0] = plI.plS.Name;
                vars[1] = forceEntry;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, forceEntry: {1:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[14], checkHooks.EXTOnPlayerVehicleEntry.ToString(), output.ToString());
            }
            return true; //If set to false, it will prevent player enter a vehicle.
        }

        [DllExport("EXTOnPlayerVehicleEject", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnPlayerVehicleEject([In] Addon_API.PlayerInfo plI, [In, MarshalAs(UnmanagedType.I1)] bool forceEject) {
            if (checkHooks.EXTOnPlayerVehicleEject < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerVehicleEject++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                vars[0] = plI.plS.Name;
                vars[1] = forceEject;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, forceEject: {1:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[15], checkHooks.EXTOnPlayerVehicleEject.ToString(), output.ToString());
            }
            return true; //If set to false, it will prevent player leave a vehicle.
        }

        [DllExport("EXTOnPlayerSpawnColor", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnPlayerSpawnColor([In] Addon_API.PlayerInfo plI, [In, MarshalAs(UnmanagedType.I1)] bool isTeamPlay) {
            if (checkHooks.EXTOnPlayerSpawnColor < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerSpawnColor++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[3];
                vars[0] = plI.plS.Name;
                vars[1] = plI.plR.ColorIndex;
                vars[2] = isTeamPlay;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, Color Index: {1:hd}, isTeamPlay: {2:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[16], checkHooks.EXTOnPlayerSpawnColor.ToString(), output.ToString());
            }
            return true; //If set to false, it will use custom color instead of team color.
        }

        [DllExport("EXTOnPlayerValidateConnect", CallingConvention = CallingConvention.Cdecl)]
        public static HEXT.PLAYER_VALIDATE EXTOnPlayerValidateConnect([In] Addon_API.PlayerExtended plEx, [In] s_machine_slot mS, [In] s_ban_check banCheckPlayer, [In, MarshalAs(UnmanagedType.I1)] bool isBanned, [In] byte svPass, [In] HEXT.PLAYER_VALIDATE isForceReject) {
            if (checkHooks.EXTOnPlayerValidateConnect < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerValidateConnect++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[6];
                string cdKeyHash = Encoding.ASCII.GetString(banCheckPlayer.cdKeyHash.str);
                vars[0] = plEx.CDHashW;
                vars[1] = cdKeyHash;
                vars[2] = banCheckPlayer.requestPlayerName;
                vars[3] = isBanned;
                vars[4] = svPass;
                vars[5] = isForceReject;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "plEx.CDHashW: {0:s}, banCheckPlayer.cdKeyHash: {1:s}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[17], checkHooks.EXTOnPlayerValidateConnect.ToString()+".1", output.ToString());
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Request Name: {2:s}, isBanned: {3:d}, svPass: {4:hhd} isForceReject: {5:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[17], checkHooks.EXTOnPlayerValidateConnect.ToString()+".2", output.ToString());
            }
            return HEXT.PLAYER_VALIDATE.DEFAULT; //Look in PLAYER_VALIDATE enum for available options to return.
        }

        [DllExport("EXTOnWeaponReload", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnWeaponReload([In] s_object_ptr obj_Struct, [In, MarshalAs(UnmanagedType.I1)] bool allowReload) {
            if (checkHooks.EXTOnWeaponReload < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnWeaponReload++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                s_object_managed obj_Struct_managed = new s_object_managed(obj_Struct);
                vars[0] = obj_Struct_managed.s_object_n.ModelTag.Tag;
                vars[1] = allowReload;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Object Model Tag: {0:08X}, allowReload: {1:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[18], checkHooks.EXTOnWeaponReload.ToString(), output.ToString());
            }
            return true; //If set to false, weapon will not reload. Unless allowReload is false, then this value is ignored.
        }

        // Enabled in 0.5.3.4
        [DllExport("EXTOnObjectCreate", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnObjectCreate([In] s_ident obj_id, [In] s_object_ptr obj_Struct, [In] Addon_API.hTagHeaderPtr header) {
            if (checkHooks.EXTOnObjectCreate < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnObjectCreate++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                s_object_managed obj_Struct_managed = new s_object_managed(obj_Struct);
                Addon_API.hTagHeader_managed header_managed = new Addon_API.hTagHeader_managed(header);
                vars[0] = obj_Struct_managed.s_object_n.ModelTag.Tag;
                vars[1] = header_managed.hTagHeader_n.tag_name;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Object Model Tag: {0:08X}, tag_name: {1:s}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[19], checkHooks.EXTOnObjectCreate.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnKillMultiplier", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnKillMultiplier([In] Addon_API.PlayerInfo killer, [In] uint multiplier) {
            if (checkHooks.EXTOnKillMultiplier < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnKillMultiplier++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                vars[0] = killer.plS.Name;
                vars[1] = multiplier;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, Multiplier: {1:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[20], checkHooks.EXTOnKillMultiplier.ToString(), output.ToString());
            }
        }

        // Enabled in 0.5.3.4
        [DllExport("EXTOnVehicleRespawnProcess", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnVehicleRespawnProcess([In] s_ident obj_id, [In] s_object_ptr cur_object, [In] Addon_API.objManagedPtr managedObj, [In, MarshalAs(UnmanagedType.I1)] bool isManaged) {
            if (checkHooks.EXTOnVehicleRespawnProcess < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnVehicleRespawnProcess++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[5];
                s_object_managed cur_object_managed = new s_object_managed(cur_object);
                Addon_API.objManaged_managed objManaged_managed = new Addon_API.objManaged_managed(managedObj);
                vars[0] = cur_object_managed.s_object_n.ModelTag.Tag;
                vars[1] = objManaged_managed.objManaged_n.world.x;
                vars[2] = objManaged_managed.objManaged_n.world.y;
                vars[3] = objManaged_managed.objManaged_n.world.z;
                vars[4] = isManaged;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "ModelTag: {0:08X}, World X: {1:f}, Y: {2:f}, Z: {3:f}, isManaged: {4:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[21], checkHooks.EXTOnVehicleRespawnProcess.ToString(), output.ToString());
            }
            return true; //If set to false, it is managed by you. True for default.
        }

        // Enabled in 0.5.3.4
        [DllExport("EXTOnObjectDeleteAttempt", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnObjectDeleteAttempt([In] s_ident obj_id, [In] s_object_ptr cur_object, [In] int curTicks, [In, MarshalAs(UnmanagedType.I1)] bool isManaged) {
            if (checkHooks.EXTOnObjectDeleteAttempt < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnObjectDeleteAttempt++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[3];
                s_object_managed cur_object_managed = new s_object_managed(cur_object);
                vars[0] = cur_object_managed.s_object_n.ModelTag.Tag;
                vars[1] = curTicks;
                vars[2] = isManaged;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "ModelTag: {0:08X}, Current Ticks: {1:d}, isManaged: {2:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[22], checkHooks.EXTOnObjectDeleteAttempt.ToString(), output.ToString());
            }
            return true; //If set to false, it is managed by you. True for default.
        }

        [DllExport("EXTOnObjectDamageLookupProcess", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnObjectDamageLookupProcess([In] Addon_API.objDamageInfoPtr damageInfo, [In] s_ident_ptr obj_recv, [In] boolOption allowDamage, [In, MarshalAs(UnmanagedType.I1)] bool isManaged) {
            if (checkHooks.EXTOnObjectDamageLookupProcess < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnObjectDamageLookupProcess++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[5];
                Addon_API.objDamageInfo_managed cur_object_managed = new Addon_API.objDamageInfo_managed(damageInfo);
                s_ident_managed obj_recv_managed = new s_ident_managed(obj_recv);
                vars[0] = cur_object_managed.objDamageInfo_n.causer.Tag;
                vars[1] = cur_object_managed.objDamageInfo_n.player_causer.Tag;
                vars[2] = obj_recv_managed.isNotNull() ? obj_recv_managed.data.Tag : 0;
                vars[3] = allowDamage.boolean;
                vars[4] = isManaged;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Causer: {0:08X}, PlayerCauser: {1:08X}, obj_recv: {2:08X}, allowDamage: {3:d}, isManaged: {4:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[23], checkHooks.EXTOnObjectDamageLookupProcess.ToString(), output.ToString());
            }
            return true; //If set to false, it is managed by you. True for default.
        }

        [DllExport("EXTOnObjectDamageApplyProcess", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnObjectDamageApplyProcess([In] Addon_API.objDamageInfoPtr damageInfo, [In] s_ident_ptr obj_recv, [In] Addon_API.objHitInfoPtr hitInfo, [In, MarshalAs(UnmanagedType.I1)] bool isBacktap, [In] boolOption allowDamage, [In, MarshalAs(UnmanagedType.I1)] bool isManaged) {
            if (checkHooks.EXTOnObjectDamageApplyProcess < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnObjectDamageApplyProcess++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[6];
                Addon_API.objDamageInfo_managed cur_object_managed = new Addon_API.objDamageInfo_managed(damageInfo);
                s_ident_managed obj_recv_managed = new s_ident_managed(obj_recv);
                vars[0] = cur_object_managed.objDamageInfo_n.causer.Tag;
                vars[1] = cur_object_managed.objDamageInfo_n.player_causer.Tag;
                vars[2] = obj_recv_managed.isNotNull() ? obj_recv_managed.data.Tag : 0;
                vars[3] = isBacktap;
                vars[4] = allowDamage.boolean;
                vars[5] = isManaged;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Causer: {0:08X}, PlayerCauser: {1:08X}, obj_recv: {2:08X}, isBacktap: {3:d}, allowDamage: {4:d}, isManaged: {5:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[24], checkHooks.EXTOnObjectDamageApplyProcess.ToString(), output.ToString());
            }
            return true; //If set to false, it is managed by you. True for default.
        }

        // Featured in 0.5.1 and newer
        // Changed in 0.5.3.3
        [DllExport("EXTOnMapLoad", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnMapLoad([In] s_ident map_tag, [In, MarshalAs(UnmanagedType.LPWStr)] string map_name, [In] HEXT.GAME_MODE game_mode) {
            if (checkHooks.EXTOnMapLoad < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnMapLoad++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[3];
                vars[0] = map_tag.Tag;
                vars[1] = map_name;
                vars[2] = game_mode;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Map Tag: {0:08X}, Map: {1:s}, Game Mode: {2:hd}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[25], checkHooks.EXTOnMapLoad.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnAIVehicleEntry", CallingConvention = CallingConvention.Cdecl)]
        public static sbyte EXTOnAIVehicleEntry([In] s_ident bipedTag, [In] s_ident vehicleTag, [In] ushort seatNum, [In] sbyte isManaged) {
            if (checkHooks.EXTOnAIVehicleEntry < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnAIVehicleEntry++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[4];
                vars[0] = bipedTag.Tag;
                vars[1] = vehicleTag.Tag;
                vars[2] = seatNum;
                vars[3] = isManaged;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "bipedTag: {0:08X}, vehicleTag: {1:08X}, seatNum: {2:hd}, isManaged: {3:hhd}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[26], checkHooks.EXTOnAIVehicleEntry.ToString(), output.ToString());
            }
            return -1; //-1 = default, 0 = prevent entry, 1 = force entry
        }
        // Changed in 0.5.3.3
        [DllExport("EXTOnWeaponDropCurrent", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnWeaponDropCurrent([In] Addon_API.PlayerInfo plI, [In] s_ident bipedTag, [In] s_object_ptr biped, [In] s_ident weaponTag, [In] s_object_ptr weapon) {
            if (checkHooks.EXTOnWeaponDropCurrent < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnWeaponDropCurrent++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                vars[0] = bipedTag.Tag;
                vars[1] = weaponTag.Tag;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "bipedTag: {0:08X}, weaponTag: {1:08X}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[27], checkHooks.EXTOnWeaponDropCurrent.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnServerStatus", CallingConvention = CallingConvention.Cdecl)]
        public static sbyte EXTOnServerStatus([In] int execId, [In] sbyte isManaged) {
            if (checkHooks.EXTOnServerStatus < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnServerStatus++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                vars[0] = execId;
                vars[1] = isManaged;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "execId: {0:d}, isManaged: {1:hhd}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[28], checkHooks.EXTOnServerStatus.ToString(), output.ToString());
            }
            return -1; //-1 = default, 0 = hide message, 1 = show message
        }

        // Featured in 0.5.2.3 and newer
        [DllExport("EXTOnPlayerUpdate", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnPlayerUpdate([In] Addon_API.PlayerInfo plI) {
            if (checkHooks.EXTOnPlayerUpdate < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnPlayerUpdate++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                vars[0] = plI.plS.Name;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[29], checkHooks.EXTOnPlayerUpdate.ToString(), output.ToString());
            }
        }

        [DllExport("EXTOnMapReset", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnMapReset() {
            if (checkHooks.EXTOnMapReset < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnMapReset++;
                pICIniFile.m_value_set(HookNames[30], checkHooks.EXTOnMapReset.ToString(), "Reset");
            }
        }

        // Featured in 0.5.3.0 and newer
        // Enabled in 0.5.3.4
        [DllExport("EXTOnObjectCreateAttempt", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnObjectCreateAttempt([In] Addon_API.PlayerInfo plOwner, [In] Addon_API.objCreationInfo object_creation, [In, Out] Addon_API.objCreationInfoPtr change_object, [In, MarshalAs(UnmanagedType.I1)] bool isOverride) {
            if (checkHooks.EXTOnObjectCreateAttempt < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnObjectCreateAttempt++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[6];
                vars[0] = object_creation.map_id.Tag;
                vars[1] = object_creation.parent_id.Tag;
                vars[2] = object_creation.pos.x;
                vars[3] = object_creation.pos.y;
                vars[4] = object_creation.pos.z;
                vars[5] = isOverride;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "map_id: {0:08X}, parent_id: {1:08X}, pos.x: {2:f}, pos.y: {3:f}, pos.z: {4:f}, isOverride: {5:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[31], checkHooks.EXTOnObjectCreateAttempt.ToString(), output.ToString());
            }
            return false; //Set to true will override. False for default.
        }

        //Featured in 0.5.3.2 and newer
        [DllExport("EXTOnGameSpyValidationCheck", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnGameSpyValidationCheck([In] uint UniqueID, [In, MarshalAs(UnmanagedType.I1)] bool isValid, [In, MarshalAs(UnmanagedType.I1)] bool forceBypass) {
            if (checkHooks.EXTOnGameSpyValidationCheck < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnGameSpyValidationCheck++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[3];
                vars[0] = UniqueID;
                vars[1] = isValid;
                vars[2] = forceBypass;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "UniqueID: {0:d}, isValid: {1:d}, forceBypass: {2:d}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[32], checkHooks.EXTOnGameSpyValidationCheck.ToString(), output.ToString());
            }
            return true; //Set to false will force bypass. True for default. Use isOverride check.
        }

        //Featured in 0.5.3.3 and newer
        [DllExport("EXTOnWeaponExchangeAttempt", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static bool EXTOnWeaponExchangeAttempt([In] Addon_API.PlayerInfo plOwner, [In] s_ident bipedTag, [In] s_object_ptr biped, [In] int slot_index, s_ident weaponTag, [In] s_object_ptr weapon, [In, MarshalAs(UnmanagedType.I1)] bool allowExchange) {
            if (checkHooks.EXTOnWeaponExchangeAttempt < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnWeaponExchangeAttempt++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[5];
                vars[0] = plOwner.cplS == IntPtr.Zero ? nullStr: plOwner.plS.Name;
                vars[1] = bipedTag.Tag;
                vars[2] = slot_index;
                vars[3] = weaponTag.Tag;
                vars[4] = allowExchange;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Player: {0:s}, bipedTag: {1:08X}, index: {2:d}, weaponTag: {3:08X}, allowExchange: {4:hhd}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[33], checkHooks.EXTOnWeaponExchangeAttempt.ToString(), output.ToString());
            }
            return true; //If false, then will deny exchange weapon. True by default. Use allowExchange check, if already false. Don't do anything.
        }

        //Featured in 0.5.3.4 and newer
        [DllExport("EXTOnObjectDelete", CallingConvention = CallingConvention.Cdecl)]
        public static void EXTOnObjectDelete([In] s_ident obj_id, [In] s_object_ptr obj_Struct, [In] Addon_API.hTagHeaderPtr header) {
            if (checkHooks.EXTOnObjectDelete < MAX_HOOK_COUNTER) {
                checkHooks.EXTOnObjectDelete++;
                StringBuilder output = new StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX);
                object[] vars = new object[2];
                s_object_managed obj_Struct_managed = new s_object_managed(obj_Struct);
                Addon_API.hTagHeader_managed header_managed = new Addon_API.hTagHeader_managed(header);
                vars[0] = obj_Struct_managed.s_object_n.ModelTag.Tag;
                vars[1] = header_managed.hTagHeader_n.tag_name;
                pIUtil.m_formatVariantW(output, (uint)output.Capacity, "Object Model Tag: {0:08X}, tag_name: {1:s}", (uint)vars.Length, vars);
                pICIniFile.m_value_set(HookNames[34], checkHooks.EXTOnObjectDelete.ToString(), output.ToString());
            }
        }
    }
}
