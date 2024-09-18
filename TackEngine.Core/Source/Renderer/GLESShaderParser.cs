using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using TackEngine.Core.Main;

namespace TackEngine.Core.Renderer {
    internal static class GLESShaderParser {
        public struct ParsedShaderDetails {
            public bool m_supportsLighting;
            public string m_lightArrayVarName;
            public string m_cameraInfoVarName;
            public int m_maxLightAmount;
        }

        public static bool VerifyShader(string vertSource, string fragSource, out ParsedShaderDetails details) {
            // First verify that it is the correct shader/glsl version
            if (!VerifyShaderVersion(vertSource, "300 es")) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error: Incorrect GLSL verison on vertex shader. Required: 300 es");
                details = new ParsedShaderDetails() { m_supportsLighting = false };
                return false;
            }

            if (!VerifyShaderVersion(fragSource, "300 es")) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error: Incorrect GLSL verison on fragment shader. Required: 300 es");
                details = new ParsedShaderDetails() { m_supportsLighting = false };
                return false;
            }

            Dictionary<string, string> variableNames = new Dictionary<string, string>();

            bool vertRes = MatchVertexShaderVariables(vertSource, out variableNames);

            if (!vertRes) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error: Incorrect variables in the vertex shader");
                details = new ParsedShaderDetails() { m_supportsLighting = false };
                return false;
            }

            bool fragRes = MatchFragmentShaderVariables(fragSource, variableNames, out details);

            if (!fragRes) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error: Incorrect variables in the fragment shader");
                details = new ParsedShaderDetails() { m_supportsLighting = false };
                return false;
            }

            return true;
        }

        public static bool MatchVertexShaderVariables(string source, out Dictionary<string, string> vertVars) {
            /* Things we need to check for in vertex shader:
             * -----------------------------
             * 
             * in vec3 aPos;
             * in vec2 aTexCoord;
             * out vec2 fTexCoord;
             * uniform mat4 uModelMat;
             */


            // in vec3 aPos;
            Match posMatch = Regex.Match(source, @"in\svec3\s(\w+);");

            vertVars = new Dictionary<string, string>();

            if (!posMatch.Success) {
                return false;
            }

            vertVars.Add("aPos", posMatch.Groups[1].Value);

            // layout (location = 1) in vec2 aTexCoord;
            Match texCoordMatch = Regex.Match(source, @"in\svec2\s(\w+);");

            if (!posMatch.Success) {
                return false;
            }

            vertVars.Add("aTexCoord", texCoordMatch.Groups[1].Value);

            // out vec2 fTexCoord;
            Match outTexCoordMatch = Regex.Match(source, @"out\svec2\s(\w+);");

            if (!outTexCoordMatch.Success) {
                return false;
            }

            vertVars.Add("fTexCoord", outTexCoordMatch.Groups[1].Value);

            // uniform mat4 uModelMat;
            Match modelMatMatch = Regex.Match(source, @"uniform\smat4\s(\w+);");

            if (!modelMatMatch.Success) {
                return false;
            }

            vertVars.Add("uModelMat", modelMatMatch.Groups[1].Value);

            return true;
        }

        public static bool MatchFragmentShaderVariables(string source, Dictionary<string, string> vertVars, out ParsedShaderDetails details) {
            /* Things we need to check for in fragement shader:
             * -----------------------------
             * 
             * out mediump vec4 FragColor;
             * in mediump vec2 fTexCoord;
             */

            // out vec4 FragColor;
            Match fragColourMatch = Regex.Match(source, @"out\smediump\svec4\sFragColor;");

            if (!fragColourMatch.Success) {
                details = new ParsedShaderDetails() { m_supportsLighting = false };
                return false;
            }

            // in vec2 fTexCoord;
            Match texCooordMatch = Regex.Match(source, @"in\smediump\svec2\s" + vertVars["fTexCoord"] + ";");

            if (!texCooordMatch.Success) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error: No matching variable in fragment shader with name '" + vertVars["fTexCoord"] + "'");
                details = new ParsedShaderDetails() { m_supportsLighting = false };
                return false;
            }

            bool camInfoMatchRes = MatchCameraInfoStructAndVariable(source, out string camInfoVariableName);
            bool lightingInfoMatchRes = MatchLightingInfoStructAndVariable(source, out string lightingInfoVariableName);

            if (camInfoMatchRes && lightingInfoMatchRes) {
                details = new ParsedShaderDetails() { m_supportsLighting = true, m_cameraInfoVarName = camInfoVariableName, m_lightArrayVarName = lightingInfoVariableName };
                return true;
            }

            if (camInfoMatchRes) {
                details = new ParsedShaderDetails() { m_supportsLighting = false, m_cameraInfoVarName = camInfoVariableName };
                return true;
            } else {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error: A fragment shader must define the CameraInfo struct and uniform variable");
                details = new ParsedShaderDetails() { m_supportsLighting = false };
                return false;
            }
        }

        private static bool MatchCameraInfoStructAndVariable(string source, out string variableName) {
            Match cameraInfoStructDef = Regex.Match(source.Replace("\r", ""), @"struct CameraInfo\s{\n\t*mediump\svec2\sposition;\n\t*mediump\svec2\ssize;\n\t*mediump\sfloat\szoomFactor;\n\t*};");
            Match cameraInfoVar = Regex.Match(source.Replace("\r", ""), @"uniform\sCameraInfo\s(\w+);");

            if (cameraInfoStructDef.Success && cameraInfoVar.Success) {
                variableName = cameraInfoVar.Groups[1].Value;
                return true;
            }

            variableName = "";
            return false;
        }

        private static bool MatchLightingInfoStructAndVariable(string source, out string variableName) {
            Match lightStructDef = Regex.Match(source.Replace("\r", ""), @"struct Light\s*{\n\t*mediump\svec2\sposition;\n\t*mediump\svec4\scolour;\n\t*mediump\sfloat\sintensity;\n\t*mediump\sfloat\sradius;\n\t*};");
            Match lightInfoStructDef = Regex.Match(source.Replace("\r", ""), @"struct LightingInfo\s{\n\t*int\slightCount;\n\t*Light\slights[\[](\d+)[\]];\n\t*mediump\svec4\sambientColour;\n\t*mediump\sfloat\sambientColourIntensity;\n\t*};");
            Match lightInfoVar = Regex.Match(source.Replace("\r", ""), @"uniform\sLightingInfo\s(\w+);");

            if (lightInfoStructDef.Success && lightStructDef.Success && lightInfoVar.Success) {
                if (Convert.ToInt32(lightInfoStructDef.Groups[1].Value) != TackLightingSystem.Instance.MaxLights) {
                    throw new Exception("Failed to verify fragment shader. The Light array must have a length equal to the maximum lights permitted. This is currently " + TackLightingSystem.Instance.MaxLights);
                }

                variableName = lightInfoVar.Groups[1].Value;
                return true;
            }

            variableName = "";
            return false;
        }

        public static bool VerifyShaderVersion(string source, string versionString) {
            Regex regex = new Regex("[#]version\\s" + versionString);

            Match m1 = regex.Match(source);

            if (m1 == null) {
                return false;
            }

            return true;
        }
    }
}
