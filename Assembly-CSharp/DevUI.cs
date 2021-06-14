﻿using System;
using UnityEngine;
using L2Base;
using L2Flag;
using LM2RandomiserMod.Patches;

namespace LM2RandomiserMod
{
    public class DevUI : MonoBehaviour
    {
        private Font currentFont = null;

        private L2System sys;

        private bool showUI = false;
        private bool showFlagWatch = false;


        private string areaString;
        private string screenXString;
        private string screenYString;
        private string posXString;
        private string posYString;
        private bool sceneJump = true;
        private BGScrollSystem currentBGSys;

        private string sheetString;
        private string flagString;
        private string valueString;

        private string getSheetString;
        private string getFlagString;
        private string getValueString;

        private string ccIdString;
        private string ccCodeString;
        private string ccTypeString;
        private string ccTimeString;
        public string ccConnected = "No CC";
        public string ccMessage;

        public void Initialise(L2System l2System)
        {
            sys = l2System;
            Cursor.visible = true;
        }

        public void OnGUI() {

            if (showUI && sys.getPlayer() != null)
            {
                areaString = GUI.TextArea(new Rect(0, 0, 100, 25), areaString);
                screenXString = GUI.TextArea(new Rect(0, 25, 50, 25), screenXString);
                screenYString = GUI.TextArea(new Rect(50, 25, 50, 25), screenYString);
                posXString = GUI.TextArea(new Rect(0, 50, 50, 25), posXString);
                posYString = GUI.TextArea(new Rect(50, 50, 50, 25), posYString);

                if (GUI.Button(new Rect(0, 75, 100, 25), "Warp"))
                {
                    DoDebugWarp();
                }

                sheetString = GUI.TextArea(new Rect(100, 0, 100, 25), sheetString);
                flagString = GUI.TextArea(new Rect(100, 25, 100, 25), flagString);
                valueString = GUI.TextArea(new Rect(100, 50, 100, 25), valueString);

                if (GUI.Button(new Rect(100, 75, 100, 25), "Set Flag"))
                {
                    SetFlag();
                }

                getSheetString = GUI.TextArea(new Rect(200, 0, 100, 25), getSheetString);
                getFlagString = GUI.TextArea(new Rect(200, 25, 100, 25), getFlagString);
                getValueString = GUI.TextArea(new Rect(200, 50, 100, 25), getValueString);

                if (GUI.Button(new Rect(200, 75, 100, 25), "Get Flag"))
                {
                    GetFlag();
                }

                ccIdString = GUI.TextArea(new Rect(400, 0, 100, 25), ccIdString);
                ccCodeString = GUI.TextArea(new Rect(400, 25, 100, 25), ccCodeString);
                ccTypeString = GUI.TextArea(new Rect(400, 50, 100, 25), ccTypeString);
                ccTimeString = GUI.TextArea(new Rect(400, 75, 100, 25), ccTimeString);
                ccConnected = GUI.TextArea(new Rect(400, 100, 100, 25), ccConnected);
                ccMessage = GUI.TextArea(new Rect(400, 125, 400, 425), ccMessage);

                sys.setPandaModeHP(GUI.Toggle(new Rect(300, 0, 120, 25), sys.getPandaModeHP(), "Panda Mode"));
                sys.setPandaModeHit(GUI.Toggle(new Rect(300, 25, 120, 25), sys.getPandaModeHit(), "Panda Hit Mode"));
            }

            if (showFlagWatch)
            {
                if (currentFont == null)
                {
                    currentFont = Font.CreateDynamicFontFromOSFont("Consolas", 14);
                }

                GUIStyle guistyle = new GUIStyle(GUI.skin.label);
                guistyle.normal.textColor = Color.white;
                guistyle.fontStyle = FontStyle.Bold;
                guistyle.font = currentFont;
                guistyle.fontSize = 14;

                var flagWatch = ((patched_L2FlagSystem)sys.getFlagSys()).GetFlagWatches();

                if (flagWatch == null)
                    return;

                guistyle.fontSize = 10;

                try
                {
                    string flags = string.Empty;

                    foreach(var flag in flagWatch)
                    {
                        flags = string.Format($"{flags}\n{flag}");
                    }
                    GUIContent flw1 = new GUIContent(flags);
                    Vector2 flw1Size = guistyle.CalcSize(flw1);
                    GUI.Label(new Rect(0, Screen.height - flw1Size.y, flw1Size.x, flw1Size.y), flw1, guistyle);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.F10))
                showUI = !showUI;

            if (Input.GetKeyDown(KeyCode.F9))
                showFlagWatch = !showFlagWatch;

            if (Input.GetKeyDown(KeyCode.F7))
                sys.drawHitBox(!sys.drawHitBoxFlag);

            UpdateBGSys();
        }
        
        public void UpdateCCMessage(int id, string code, int type)
        {
            this.ccIdString = id.ToString();
            this.ccCodeString = code;
            this.ccTypeString = type.ToString();
            this.ccTimeString = System.DateTime.Now.ToString();
        }

        private void UpdateBGSys()
        {
            if (sceneJump)
            {
                currentBGSys = sys.getL2SystemCore().ScrollSystem;
                if (currentBGSys != null)
                {
                    sceneJump = false;
                    UpdatePositionInfo();
                }
            }
        }

        private void UpdatePositionInfo()
        {
            L2SystemCore sysCore = sys.getL2SystemCore();
            GameObject playerObj = sys.getPlayer().gameObject;
            Vector3 position = playerObj.transform.position;
            ViewProperty currentView = currentBGSys.roomSetter.getCurrentView(position.x, position.y);
            int currentScene = sysCore.SceaneNo;
            areaString = currentScene.ToString();
            float num;
            float num2;
            if (currentView == null)
            {
                screenXString = "-1";
                screenYString = "-1";
                num = 0f;
                num2 = 0f;
            }
            else
            {
                screenXString = currentView.ViewX.ToString();
                screenYString = currentView.ViewY.ToString();
                num = position.x - currentView.ViewLeft;
                num2 = position.y - currentView.ViewBottom;
            }
            int num3 = (int)Mathf.Round(num * (float)BGAbstractScrollController.NumberCls);
            int num4 = (int)Mathf.Round(num2 * (float)BGAbstractScrollController.NumberCls);
            num3 /= 80;
            num4 /= 80;
            posXString = num3.ToString();
            posYString = num4.ToString();
        }

        private void SetFlag()
        {
            int sheet = int.Parse(sheetString);
            int flag = int.Parse(flagString);
            short value = short.Parse(valueString);

            sys.setFlagData(sheet, flag, value);
        }

        private void GetFlag()
        {
            int sheet = int.Parse(getSheetString);
            int flag = int.Parse(getFlagString);
            sys.getFlagSys().getFlagBaseObject(sheet, flag, out L2FlagBase l2Flag);
            getValueString = l2Flag.flagValue.ToString();
        }

        private void DoDebugWarp()
        {
            try
            {
                int area = int.Parse(areaString);
                int screenX = int.Parse(screenXString);
                int screenY = int.Parse(screenYString);
                int posX = int.Parse(posXString);
                int posY = int.Parse(posYString);

                L2SystemCore sysCore = sys.getL2SystemCore();

                sysCore.setJumpPosition(screenX, screenY, posX, posY, 0f);
                if (sysCore.SceaneNo != area)
                {
                    sysCore.gameScreenFadeOut(10);
                    sysCore.setFadeInFlag(true);
                    sysCore.changeFieldSceane(area, true, false);
                    sceneJump = true;
                }
                else
                {
                    JumpPosition();
                    UpdatePositionInfo();
                }
            }
            catch (Exception) { }
        }

        private void JumpPosition()
        {
            if (sceneJump)
                return;

            L2SystemCore sysCore = sys.getL2SystemCore();
            if (sysCore.getJumpPosition(out Vector3 vector))
            {
                sysCore.L2Sys.movePlayer(vector);
                currentBGSys.setPlayerPosition(vector, false);
                sysCore.resetFairy();
                currentBGSys.forceResetCameraPosition();
            }
        }
    }
}
