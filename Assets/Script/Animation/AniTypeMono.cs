using UnityEditor;
using UnityEngine;

public class AniTypeMono : MonoBehaviour
{
    [SerializeField]
    private ObjectType _objectType;
    [SerializeField]
    private PlayerTypeIndex playerIndex;
    [SerializeField]
    private EnemyTypeIndex enemyIndex;
    [SerializeField]
    private ItemTypeIndex itemIndex;
    //private string itemHideValue; // item type index enum 이 수정 시 인스펙터상의 값을 보정해줌.
    [SerializeField]
    private PlayerBuildingTypeIndex playerBuildingIndex;
    [SerializeField]
    private EnemyBuildingTypeIndex enemyBuildingIndex;
    public ObjectType objectType
    {
        get { return _objectType; }
    }
    public int characterIndex
    {
        get
        {
            if (_objectType == ObjectType.Player)
                return (int)playerIndex;
            else if (_objectType == ObjectType.Enemy)
                return (int)enemyIndex;
            else if (_objectType == ObjectType.Item)
                return (int)itemIndex;
            else if (_objectType == ObjectType.PlayerBuilding)
                return (int)playerBuildingIndex;
            else if (_objectType == ObjectType.EnemyBuilding)
                return (int)enemyBuildingIndex;

            return characterIndex;
        }
    }

    [CustomEditor(typeof(AniTypeMono))]
    public class AniTypeEditor : Editor
    {
        //private AniTypeMono aniTypeMono;

        private SerializedProperty _objectType;
        private SerializedProperty playerIndex;
        private SerializedProperty enemyIndex;
        private SerializedProperty itemIndex;
        private SerializedProperty playerBuildingIndex;
        private SerializedProperty enemyBuildingIndex;
        private void OnEnable()
        {
            _objectType = serializedObject.FindProperty("_objectType");
            playerIndex = serializedObject.FindProperty("playerIndex");
            enemyIndex = serializedObject.FindProperty("enemyIndex");
            itemIndex = serializedObject.FindProperty("itemIndex");
            playerBuildingIndex = serializedObject.FindProperty("playerBuildingIndex");
            enemyBuildingIndex = serializedObject.FindProperty("enemyBuildingIndex");

            // item type index enum 이 수정 시 인스펙터상의 값을 보정해줌.
            /*
            aniTypeMono = target as AniTypeMono;
            if (aniTypeMono.itemHideValue == null || aniTypeMono.itemHideValue == string.Empty)
                aniTypeMono.itemHideValue = itemIndex.GetEnumName<ItemTypeIndex>();

            ItemTypeIndex storedValue = Enum.Parse<ItemTypeIndex>(aniTypeMono.itemHideValue);
            if ((int)storedValue != itemIndex.enumValueIndex)
                aniTypeMono.itemIndex = storedValue; */
        }
        public override void OnInspectorGUI()
        {
            // load real target values into SerializedProperties
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_objectType);

            if (_objectType.enumValueIndex == (int)ObjectType.Player)
            {
                EditorGUILayout.PropertyField(playerIndex);
            }
            else if (_objectType.enumValueIndex == (int)ObjectType.Enemy)
            {
                EditorGUILayout.PropertyField(enemyIndex);
            }
            else if (_objectType.enumValueIndex == (int)ObjectType.Item)
            {
                EditorGUILayout.PropertyField(itemIndex);
            }
            else if (_objectType.enumValueIndex == (int)ObjectType.PlayerBuilding)
            {
                EditorGUILayout.PropertyField(playerBuildingIndex);
            }
            else if (_objectType.enumValueIndex == (int)ObjectType.EnemyBuilding)
            {
                EditorGUILayout.PropertyField(enemyBuildingIndex);
            }

            // item type index enum 이 수정 시 인스펙터상의 값을 보정해줌.
            /*
            if(EditorGUI.EndChangeCheck())
                aniTypeMono.itemHideValue = itemIndex.GetEnumName<ItemTypeIndex>();*/

            serializedObject.ApplyModifiedProperties();
        }
    }
}