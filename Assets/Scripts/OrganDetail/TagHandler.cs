using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagHandler : MonoBehaviour
{
    private static TagHandler instance;
    public static TagHandler Instance
    {
        get {
            if (instance == null)
            {
                instance = FindObjectOfType<TagHandler>();
            }
            return instance;
        }
    }
    public TextAsset textJSON;

    public List<GameObject> addedTags = new List<GameObject>();
    [System.Serializable]
    public class Point
    {
        public Vector3 coordinate { get; set; }  // tọa độ
        public Vector3 direction { get; set; }   // hướng 
        public float angle { get; set; }   // góc
        public Point(Vector3 coordinate, Vector3 direction)
        {
            this.coordinate = coordinate;
            this.direction  = direction;
        }
    }

    [System.Serializable]
    public class Tag
    {
        public string name { get; set; }
        public string description { get; set; }
        public Point point { get; set; }
        public Vector3 tag { get; set; }
        public Tag[] child { get; set; }
        public Tag(string name, string description, Point point, Vector3 tag, Tag[] child)
        {
            this.name = name;
            this.description  = description;
            this.point  = point;
            this.tag  = tag;
            this.child  = child;
        }
    }

    [System.Serializable]
    public class Atlas
    {
        public Tag[] tags;
    }
    
    public Atlas atlas = new Atlas();

    void Start()
    {
        initAtlas();
        // loadTags();
    }
    void Update()
    {
        // if (addedTags.Count > 0)
        // {
        //     OnMove();
        // }
    }
    public void initAtlas()
    {
        atlas.tags = new Tag[]
        {
            new Tag(
                "Axial",
                "",
                new Point(
                    new Vector3(0.0f, 5.0f, 0.0f),
                    new Vector3(0.0f, 0.0f, 359.9f)
                ),
                new Vector3(0.0f, 5.5f, 0.0f),
                new Tag[]{}
            ),
            new Tag(
                "Appendicular",
                "",
                new Point(
                    new Vector3(-1.5f, 1.75f, 0.4f),
                    new Vector3(0.0f, 0.0f, 180.0f)
                ),
                new Vector3(-2.0f, 1.75f, 0.0f),
                new Tag[]{}
            ),
            new Tag(
                "Skull",
                "",
                new Point(
                    new Vector3(0.4f, 4.5f, 0.3f),
                    new Vector3(0.0f, 270.0f, 90.0f)
                ),
                new Vector3(1.5f, 4.5f, 0.4f),
                new Tag[]{}
            ),
            new Tag(
                "Vertebral Colum",
                "",
                new Point(
                    new Vector3(0.0f, 3.2f, 0.6f),
                    new Vector3(0.0f, 180.0f, 90.0f)
                ),
                new Vector3(0.0f, 3.2f, 2.0f),
                new Tag[]{}
            ),
            new Tag(
                "Thoracic Cage",
                "",
                new Point(
                    new Vector3(-0.6f, 2.0f, -0.5f),
                    new Vector3(0.0f, 0.0f, 90.0f)
                ),
                new Vector3(-0.5f, 2.2f, -0.8f),
                new Tag[]{}
            ),
            new Tag(
                "Upper Limbs",
                "",
                new Point(
                    new Vector3(1.5f, 1.75f, 0.4f),
                    new Vector3(0.0f, 270.0f, 90.0f)
                ),
                new Vector3(2.2f, 0.32f, 0.1f),
                new Tag[]{}
            ),
            new Tag(
                "Shoulder gridles",
                "",
                new Point(
                    new Vector3(-1.0f, 3.5f, 0.2f),
                    new Vector3(0.0f, 90.0f, 90.0f)
                ),
                new Vector3(-1.4f, 3.5f, 0.2f),
                new Tag[]{}
            ),
            new Tag(
                "Lower Limbs",
                "",
                new Point(
                    new Vector3(0.6f, -1.72f, 0.2f),
                    new Vector3(0.0f, 270.0f, 90.0f)
                ),
                new Vector3(1.9f, -2.2f, 0.2f),
                new Tag[]{}
            ),
            new Tag(
                "Pelvic Girdlle",
                "",
                new Point(
                    new Vector3(-0.3f, 1.16f, 0.4f),
                    new Vector3(0.0f, 180.0f, 100.0f)
                ),
                new Vector3(0.0f, 0.4f, 1.65f),
                new Tag[]{}
            )
        };
    }
    
    public void loadTags()
    {
        int i = 0;
        foreach(Tag tag in atlas.tags)
        {
            addedTags.Add(Instantiate(Resources.Load("tag") as GameObject));
            adjustTag(i, tag);
            i++;
        }
    }

    public void adjustTag(int idx, Tag tag)
    {
        addedTags[idx].transform.GetChild(0).transform.position = tag.point.coordinate;
        addedTags[idx].transform.GetChild(0).transform.eulerAngles = tag.point.direction;
        addedTags[idx].transform.GetChild(0).gameObject.GetComponent<LineRenderer>().SetVertexCount(2);
        addedTags[idx].transform.GetChild(0).gameObject.GetComponent<LineRenderer>().SetPosition(0,tag.point.coordinate);
        addedTags[idx].transform.GetChild(0).gameObject.GetComponent<LineRenderer>().SetPosition(1,tag.tag);
        addedTags[idx].transform.GetChild(0).gameObject.GetComponent<LineRenderer>().SetWidth(0.01f, 0.01f);

        addedTags[idx].transform.GetChild(1).transform.position = tag.tag;

        float scaleX = (tag.name.Length/9f);
        int scaleY = (int)(tag.name.Length / 18);
        if (scaleX > 2)
            scaleX = 2;
        if (scaleY * 18 < tag.name.Length)
            scaleY++;
        addedTags[idx].transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshPro>().SetText(tag.name);
        addedTags[idx].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(  scaleX * addedTags[idx].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta[0], 
                                                                                                                scaleY * addedTags[idx].transform.GetChild(1).gameObject.GetComponent<RectTransform>().sizeDelta[1]);
        addedTags[idx].transform.GetChild(2).transform.localScale = new Vector3(scaleX * addedTags[idx].transform.GetChild(2).transform.localScale.x, 
                                                                                scaleY * addedTags[idx].transform.GetChild(2).transform.localScale.y, 
                                                                                addedTags[idx].transform.GetChild(2).transform.localScale.z);
        addedTags[idx].transform.GetChild(2).transform.position = tag.tag;
    }

    public void OnMove()
    {
        foreach(GameObject addedTag in addedTags)
        {
            denoteTag(addedTag);
            moveTag(addedTag);
        }
    }
    
    public void denoteTag(GameObject addedTag)
    {
        if (Mathf.Abs(addedTag.transform.GetChild(2).transform.eulerAngles.y - addedTag.transform.GetChild(0).transform.eulerAngles.y) > addedTag.transform.GetChild(0).transform.eulerAngles.z)
        {
            addedTag.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = false;
            addedTag.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = false;
            addedTag.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            addedTag.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = true;
            addedTag.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = true;
            addedTag.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void moveTag(GameObject addedTag)
    {
        addedTag.transform.GetChild(1).transform.LookAt(addedTag.transform.GetChild(1).position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        addedTag.transform.GetChild(2).transform.LookAt(addedTag.transform.GetChild(2).position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}