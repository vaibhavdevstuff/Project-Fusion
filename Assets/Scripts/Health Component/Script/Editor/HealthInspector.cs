using UnityEngine;
using UnityEditor;
using DC.HealthSystem;

namespace DC.DCEditor
{
    [CustomEditor(typeof(Health))]
    internal class HealthInspector : Editor
    {
        private Health health;

        private bool showDeath = true;
        private bool showAudio = true;
        private bool showEvents = true;

        private const string DEATH_FOLDOUT_KEY = "HealthInspector.DeathFoldout";
        private const string AUDIO_FOLDOUT_KEY = "HealthInspector.AudioFoldout";
        private const string EVENTS_FOLDOUT_KEY = "HealthInspector.EventsFoldout";


        private void OnEnable()
        {
            health = (Health)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHealthIndicatorHeader();
            DrawHealthValueFields();
            DrawInvincibleField();
            DrawDeathFields();
            DrawAudioFields();
            DrawEventFields();

            serializedObject.ApplyModifiedProperties();

        }

        private void DrawHealthIndicatorHeader()
        {
            EditorGUILayout.Space();

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 20;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.alignment = TextAnchor.MiddleCenter;

            if (EditorGUIUtility.isProSkin)
                headerStyle.normal.textColor = Color.white;
            else
                headerStyle.normal.textColor = Color.black;

            Rect headerRect = EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.Space();

            //Head
            EditorGUILayout.LabelField("Current Health: " + health.value.ToString("0.00"), headerStyle);

            EditorGUILayout.Space();

            //Health Bar
            Rect lineRect = GUILayoutUtility.GetRect(18f, 10f);
            EditorGUI.ProgressBar(lineRect, health.value / health.MaxValue, "");

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Check if the user double-clicked on the header box
            if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && headerRect.Contains(Event.current.mousePosition))
            {
                // Open the script associated with this health indicator
                MonoScript script = MonoScript.FromMonoBehaviour(health);
                AssetDatabase.OpenAsset(script);
                Event.current.Use();
            }
        }

        private void DrawHealthValueFields()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxValue"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minValue"));
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(health.value)));

            EditorGUILayout.EndVertical();
        }

        private void DrawInvincibleField()
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("invincible"));
            if (health.Invincible)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("timeInvincibleAfterSpawn"));
                EditorGUI.indentLevel--;
            }
        }

        private void DrawDeathFields()
        {
            EditorGUILayout.Space();

            showDeath = EditorPrefs.GetBool(DEATH_FOLDOUT_KEY, true);
            showDeath = EditorGUILayout.Foldout(showDeath, "Death");
            EditorPrefs.SetBool(DEATH_FOLDOUT_KEY, showDeath);

            if (showDeath)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("onDeathDoSelf"));

                if (health.OnDeathDoSelf != Health.OnDeathSelf.None)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("afterDeathDelay"), new GUIContent("Delay"));

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnObjectsOnDeath"), true);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("destroyObjectsOnDeath"), true);

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawAudioFields()
        {
            EditorGUILayout.Space();

            showAudio = EditorPrefs.GetBool(AUDIO_FOLDOUT_KEY, true);
            showAudio = EditorGUILayout.Foldout(showAudio, "Audio");
            EditorPrefs.SetBool(AUDIO_FOLDOUT_KEY, showAudio);

            if (showAudio)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableAudio"), new GUIContent("Enable"));


                if (health.EnableAudio)
                {                    
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSource"));

                    EditorGUI.indentLevel++;

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("healAudio"), new GUIContent("Heal"));

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("damageAudio"), new GUIContent("Damage"));

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("deathAudio"), new GUIContent("Death"));

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();
            }

        }

        private void DrawEventFields()
        {
            EditorGUILayout.Space();

            showEvents = EditorPrefs.GetBool(EVENTS_FOLDOUT_KEY, true);
            showEvents = EditorGUILayout.Foldout(showEvents, "Events");
            EditorPrefs.SetBool(EVENTS_FOLDOUT_KEY, showEvents);

            if (showEvents)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnHealEvent"), new GUIContent("On Heal Event (float)"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnDamageEvent"), new GUIContent("On Damage Event (float)"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnDeathEvent"), new GUIContent("On Death Event (float)"));

                EditorGUILayout.EndVertical();
            }
        }


    }
}
