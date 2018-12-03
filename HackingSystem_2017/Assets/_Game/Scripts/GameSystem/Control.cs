using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace HackingSystem
{

    /// <summary>
    /// 输入信息类
    /// </summary>
    static class Control
    {
        //forward button
        public static KeyCode UpCode = KeyCode.W;
        public static bool UpValid = true;
        public static bool UpArrowDown()
        {
            return Input.GetKeyDown(UpCode);//   return Input.GetKeyDown(KeyCode.W);
        }
        public static bool UpArrowPress()
        {
            return Input.GetKey(UpCode);
        }
        public static bool UpArrowUp()
        {
            return Input.GetKeyUp(UpCode);
        }
        //backward button
        public static KeyCode DownCode = KeyCode.S;
        public static bool DownArrowDown()
        {
            return Input.GetKeyDown(DownCode);
        }
        public static bool DownArrowPress()
        {
            return Input.GetKey(DownCode);
        }
        public static bool DownArrowUp()
        {
            return Input.GetKeyUp(DownCode);
        }

        //left
        public static KeyCode LeftCode = KeyCode.A;
        public static bool LeftArrowDown()
        {
            return Input.GetKeyDown(LeftCode);
        }
        public static bool LeftArrowPress()
        {
            return Input.GetKey(LeftCode);
        }
        public static bool LeftArrowUp()
        {
            return Input.GetKeyUp(LeftCode);
        }

        //right
        public static KeyCode RightCode = KeyCode.D;
        public static bool RightArrowDown()
        {
            return Input.GetKeyDown(RightCode);
        }
        public static bool RightArrowPress()
        {
            return Input.GetKey(RightCode);
        }
        public static bool RightArrowUp()
        {
            return Input.GetKeyUp(RightCode);
        }

        public static Vector2 GetMove()
        {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        //MainWeaponL
        public static int LeftMouse = 0;
        public static bool MainWPAttackLDown()
        {
            return Input.GetMouseButtonDown(LeftMouse) && !Input.GetKey(SubWeaponSwitch);
        }
        public static bool MainWPAttackLPress()
        {
            return Input.GetMouseButton(LeftMouse) && !Input.GetKey(SubWeaponSwitch);
        }
        public static bool MainWPAttackLUp()
        {
            return Input.GetMouseButtonUp(LeftMouse) && !Input.GetKey(SubWeaponSwitch);
        }

        //MainWeaponR
        public static int RightMouse = 1;
        public static bool MainWPAttackRDown()
        {
            return Input.GetMouseButtonDown(RightMouse) && !Input.GetKey(SubWeaponSwitch);
        }
        public static bool MainWPAttackRPress()
        {
            return Input.GetMouseButton(RightMouse) && !Input.GetKey(SubWeaponSwitch);
        }
        public static bool MainWPAttackRUp()
        {
            return Input.GetMouseButtonUp(RightMouse) && !Input.GetKey(SubWeaponSwitch);
        }

        //SubWeaponL
        public static bool SubWPAttackLDown()
        {
            return Input.GetMouseButtonDown(LeftMouse) && Input.GetKey(SubWeaponSwitch);
        }
        public static bool SubWPAttackLPress()
        {
            return Input.GetMouseButton(LeftMouse) && Input.GetKey(SubWeaponSwitch);
        }
        public static bool SubWPAttackLUp()
        {
            return Input.GetMouseButtonUp(LeftMouse) && Input.GetKey(SubWeaponSwitch);
        }

        //SubWeaponR
        public static bool SubWPAttackRDown()
        {
            return Input.GetMouseButtonDown(RightMouse) && Input.GetKey(SubWeaponSwitch);
        }
        public static bool SubWPAttackRPress()
        {
            return Input.GetMouseButton(RightMouse) && Input.GetKey(SubWeaponSwitch);
        }
        public static bool SubWPAttackRUp()
        {
            return Input.GetMouseButtonUp(RightMouse) && Input.GetKey(SubWeaponSwitch);
        }

        //jump
        public static KeyCode JumpCode = KeyCode.Space;
        public static bool JumpArrowDown()
        {
            return Input.GetKeyDown(JumpCode);
        }
        public static bool JumpArrowPress()
        {
            return Input.GetKey(JumpCode);
        }
        public static bool JumpArrowUp()
        {
            return Input.GetKeyUp(JumpCode);
        }

        //run
        public static KeyCode RunCode = KeyCode.LeftShift;
        public static bool RunArrowDown()
        {
            return Input.GetKeyDown(RunCode);
        }
        public static bool RunArrowPress()
        {
            return Input.GetKey(RunCode);
        }
        public static bool RunArrowUp()
        {
            return Input.GetKeyUp(RunCode);
        }

        //back skill
        public static KeyCode BackCode = KeyCode.Q;
        public static bool BackArrowDown()
        {
            return Input.GetKeyDown(BackCode);
        }
        public static bool BackArrowPress()
        {
            return Input.GetKey(BackCode);
        }
        public static bool BackArrowUp()
        {
            return Input.GetKeyUp(BackCode);
        }

        //core skill
        public static KeyCode CoreCode = KeyCode.E;
        public static bool CoreArrowDown()
        {
            return Input.GetKeyDown(CoreCode);
        }
        public static bool CoreArrowPress()
        {
            return Input.GetKey(CoreCode);
        }
        public static bool CoreArrowUp()
        {
            return Input.GetKeyUp(CoreCode);
        }
        
        public static KeyCode SubWeaponSwitch = KeyCode.F;

        //menu 
        public static KeyCode MenuCode = KeyCode.B;
        public static bool MenuArrowDown()
        {
            return Input.GetKeyDown(MenuCode);
        }
        public static bool MenuArrowPress()
        {
            return Input.GetKey(MenuCode);
        }
        public static bool MenuArrowUp()
        {
            return Input.GetKeyUp(MenuCode);
        }

        // energybag
        public static KeyCode EnergyBagCode = KeyCode.Alpha1;
        public static bool EnergyBagArrowDown()
        {
            return Input.GetKeyDown(EnergyBagCode);
        }
        public static bool EnergyBagPress()
        {
            return Input.GetKey(EnergyBagCode);
        }
        public static bool EnergyBagArrowUp()
        {
            return Input.GetKeyUp(EnergyBagCode);
        }

        //Gpbag
        public static KeyCode GpBagCode = KeyCode.Alpha2;
        public static bool GpBagArrowDown()
        {
            return Input.GetKeyDown(GpBagCode);
        }
        public static bool GpBagPress()
        {
            return Input.GetKey(GpBagCode);
        }
        public static bool GpBagArrowUp()
        {
            return Input.GetKeyUp(GpBagCode);
        }

        //NuclearWaste
        public static KeyCode NuclearWasteCode = KeyCode.Alpha3;
        public static bool NuclearWasteArrowDown()
        {
            return Input.GetKeyDown(NuclearWasteCode);
        }
        public static bool NuclearWastePress()
        {
            return Input.GetKey(NuclearWasteCode);
        }
        public static bool NuclearWasteArrowUp()
        {
            return Input.GetKeyUp(NuclearWasteCode);
        }

        //tool4
        public static KeyCode ToolFourCode = KeyCode.Alpha4;
        public static bool ToolFourArrowDown()
        {
            return Input.GetKeyDown(ToolFourCode);
        }
        public static bool ToolFourPress()
        {
            return Input.GetKey(ToolFourCode);
        }
        public static bool ToolFourArrowUp()
        {
            return Input.GetKeyUp(ToolFourCode);
        }

        //tool5
        public static KeyCode ToolFiveCode = KeyCode.Alpha5;
        public static bool ToolFiveArrowDown()
        {
            return Input.GetKeyDown(ToolFiveCode);
        }
        public static bool ToolFivePress()
        {
            return Input.GetKey(ToolFiveCode);
        }
        public static bool ToolFiveArrowUp()
        {
            return Input.GetKeyUp(ToolFiveCode);
        }

        //tool6
        public static KeyCode ToolSixCode = KeyCode.Alpha6;
        public static bool ToolSixArrowDown()
        {
            return Input.GetKeyDown(ToolSixCode);
        }
        public static bool ToolSixPress()
        {
            return Input.GetKey(ToolSixCode);
        }
        public static bool ToolSixArrowUp()
        {
            return Input.GetKeyUp(ToolSixCode);
        }

        //Look around
        public static KeyCode LookAroundCode = KeyCode.LeftAlt;
        public static bool LookAroundArrowDown()
        {
            return Input.GetKeyDown(LookAroundCode);
        }
        public static bool LookAroundPress()
        {
            return Input.GetKey(LookAroundCode);
        }
        public static bool LookAroundArrowUp()
        {
            return Input.GetKeyUp(LookAroundCode);
        }

        //Tab
        public static KeyCode TabCode = KeyCode.Tab;
        public static bool TabArrowDown()
        {
            return Input.GetKeyDown(TabCode);
        }
        public static bool TabPress()
        {
            return Input.GetKey(TabCode);
        }
        public static bool TabArrowUp()
        {
            return Input.GetKeyUp(TabCode);
        }

        //exit hackin
        public static KeyCode ExitHackCode = KeyCode.I;
        public static bool ExitHackDown()
        {
            return Input.GetKeyDown(ExitHackCode);
        }
        public static bool ExitHackPress()
        {
            return Input.GetKey(ExitHackCode);
        }
        public static bool ExitHackArrowUp()
        {
            return Input.GetKeyUp(ExitHackCode);
        }

        //shift hackin
        public static KeyCode ShiftHackCode = KeyCode.K;
        public static bool ShiftHackDown()
        {
            return Input.GetKeyDown(ShiftHackCode);
        }
        public static bool ShiftHackPress()
        {
            return Input.GetKey(ShiftHackCode);
        }
        public static bool ShiftHackArrowUp()
        {
            return Input.GetKeyUp(ShiftHackCode);
        }

    }
}
