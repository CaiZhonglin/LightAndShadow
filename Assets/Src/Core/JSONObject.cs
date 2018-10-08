#define PRETTY		//Comment out when you no longer need to read JSON to disable pretty Print system-wide
//Using doubles will cause errors in VectorTemplates.cs; Unity speaks floats
#define USEFLOAT	//Use floats for numbers instead of doubles	(enable if you're getting too many significant digits in string output)
//#define POOLING	//Currently using a build setting for this one (also it's experimental)
using System.Diagnostics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Debug = UnityEngine.Debug;

/*
 * http://www.opensource.org/licenses/lgpl-2.1.php
 * JSONObject class
 * for use with Unity
 * Copyright Matt Schoen 2010 - 2013
 */

public class JSONObject
{
	#if POOLING
	const int MAX_POOL_SIZE = 10000;
public static Queue<JSONObject> releaseQueue = new Queue<JSONObject>();
#endif

	const int MAX_DEPTH = 100;
	const string INFINITY = "\"INFINITY\"";
	const string NEGINFINITY = "\"NEGINFINITY\"";
	const string NaN = "\"NaN\"";
	static readonly char[] WHITESPACE = new[] { ' ', '\r', '\n', '\t' };

	public enum Type
	{
		NULL,
		STRING,
		NUMBER,
		LONG,
		OBJECT,
		ARRAY,
		BOOL,
		BAKED
	}

	public bool isContainer { get { return (type == Type.ARRAY || type == Type.OBJECT); } }

	public Type type = Type.NULL;

	public int Count {
		get {
			if (list == null)
				return -1;
			return list.Count;
		}
	}

	public List<JSONObject> list;
	public List<string> keys;
	public string str;
    public object obj;
	#if USEFLOAT
	long nValue;
	float fValue;

	public long n {
		get { return nValue; }
		set {
			nValue = value;
			fValue = nValue;
			isIntNumber = true;
		}
	}

	public int i { get { return (int)n; } }

	public long l { get { return n; } set { n = value; } }

	public float f {
		get { return fValue; }
		set {
			fValue = value;
			nValue = (int)fValue;
			isIntNumber = false;
		}
	}

	public bool isIntNumber	{	get; private set; }

	#else
	public double n;
	public float f {
		get {
			return (float)n;
		}
	}
	#endif
	public bool b;

	public delegate void AddJSONConents (JSONObject self);

	public static JSONObject nullJO { get { return Create (Type.NULL); } }
	//an empty, null object
	public static JSONObject newObj { get { return Create (Type.OBJECT); } }
	//an empty object
	public static JSONObject arr { get { return Create (Type.ARRAY); } }
	//an empty array

	public JSONObject (Type t)
	{
		type = t;
		switch (t) {
		case Type.ARRAY:
			list = new List<JSONObject> ();
			break;
		case Type.OBJECT:
			list = new List<JSONObject> ();
			keys = new List<string> ();
			break;
		}
	}

	public JSONObject (bool b)
	{
		type = Type.BOOL;
		this.b = b;
	}

	public JSONObject (long l)
	{
		type = Type.LONG;
		this.l = l;
	}
	#if USEFLOAT
	public JSONObject (float f)
	{
		type = Type.NUMBER;
		this.f = f;
	}
	#else
	public JSONObject(double d) {
		type = Type.NUMBER;
		n = d;
	}
	#endif
	public JSONObject (Dictionary<string, string> dic)
	{
		type = Type.OBJECT;
		keys = new List<string> ();
		list = new List<JSONObject> ();
		//Not sure if it's worth removing the foreach here
		foreach (KeyValuePair<string, string> kvp in dic) {
			keys.Add (kvp.Key);
			list.Add (CreateStringObject (kvp.Value));
		}
	}

	public JSONObject (Dictionary<string, JSONObject> dic)
	{
		type = Type.OBJECT;
		keys = new List<string> ();
		list = new List<JSONObject> ();
		//Not sure if it's worth removing the foreach here
		foreach (KeyValuePair<string, JSONObject> kvp in dic) {
			keys.Add (kvp.Key);
			list.Add (kvp.Value);
		}
	}

	public JSONObject (AddJSONConents content)
	{
		content.Invoke (this);
	}

	public JSONObject (JSONObject[] objs)
	{
		type = Type.ARRAY;
		list = new List<JSONObject> (objs);
	}
	//Convenience function for creating a JSONObject containing a string.  This is not part of the constructor so that malformed JSON data doesn't just turn into a string object
	public static JSONObject StringObject (string val)
	{
		return CreateStringObject (val);
	}

	public void Absorb (JSONObject obj)
	{
		list.AddRange (obj.list);
		keys.AddRange (obj.keys);
		str = obj.str;
		#if USEFLOAT
		nValue = obj.nValue;
		fValue = obj.fValue;
		#else
		n = obj.n;
		#endif
		b = obj.b;
		type = obj.type;
	}

	public static JSONObject Create ()
	{
#if POOLING
	JSONObject result = null;
	while(result == null && releaseQueue.Count > 0) {
		result = releaseQueue.Dequeue();
#if DEV
		//The following cases should NEVER HAPPEN (but they do...)
		if(result == null)
			Debug.Log("wtf " + releaseQueue.Count);
		else if(result.list != null)
			Debug.Log("wtflist " + result.list.Count);
#endif
	}
	if(result != null)
		return result;
#endif
		return new JSONObject ();
	}

	public static JSONObject Create (Type t)
	{
		JSONObject Obj = Create ();
		Obj.type = t;
		switch (t) {
		case Type.ARRAY:
			Obj.list = new List<JSONObject> ();
			break;
		case Type.OBJECT:
			Obj.list = new List<JSONObject> ();
			Obj.keys = new List<string> ();
			break;
		}
		return Obj;
	}

	public static JSONObject Create (bool val)
	{
		JSONObject Obj = Create ();
		Obj.type = Type.BOOL;
		Obj.b = val;
		return Obj;
	}

	public static JSONObject Create (float val)
	{
		JSONObject Obj = Create ();
		Obj.type = Type.NUMBER;
		#if USEFLOAT
		Obj.f = val;
		#else
		Obj.n = val;
		#endif
		return Obj;
	}

    public static JSONObject CreateObj(object val)
    {
        JSONObject obj = Create();
        obj.type = Type.OBJECT;
        obj.obj = val;
        return  obj;
    }
	public static JSONObject Create (int val)
	{
		JSONObject Obj = Create ();
		Obj.type = Type.NUMBER;
		Obj.n = val;
		return Obj;
	}

    public static JSONObject Create(long val)
    {
        JSONObject Obj = Create();
        Obj.type = Type.NUMBER;
        Obj.n = val;
        return Obj;
    }

    public static JSONObject CreateStringObject (string val)
	{
		JSONObject Obj = Create ();
		Obj.type = Type.STRING;
		Obj.str = val;
		return Obj;
	}

	public static JSONObject CreateBakedObject (string val)
	{
		JSONObject bakedObject = Create ();
		bakedObject.type = Type.BAKED;
		bakedObject.str = val;
		return bakedObject;
	}

	/// <summary>
	/// Create a JSONObject by parsing string data
	/// </summary>
	/// <param name="val">The string to be parsed</param>
	/// <param name="maxDepth">The maximum depth for the parser to search.  Set this to to 1 for the first level, 
	/// 2 for the first 2 levels, etc.  It defaults to -2 because -1 is the depth value that is parsed (see below)</param>
	/// <param name="storeExcessLevels">Whether to store levels beyond maxDepth in baked JSONObjects</param>
	/// <param name="strict">Whether to be strict in the parsing. For example, non-strict parsing will successfully 
	/// parse "a string" into a string-type </param>
	/// <returns></returns>
	public static JSONObject Create (string val, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
	{
		JSONObject Obj = Create ();
		Obj.Parse (val, maxDepth, storeExcessLevels, strict);
		return Obj;
	}

	public static JSONObject Create (AddJSONConents content)
	{
		JSONObject Obj = Create ();
		content.Invoke (Obj);
		return Obj;
	}

	public static JSONObject Create (Dictionary<string, string> dic)
	{
		JSONObject Obj = Create ();
		Obj.type = Type.OBJECT;
		Obj.keys = new List<string> ();
		Obj.list = new List<JSONObject> ();
		//Not sure if it's worth removing the foreach here
		foreach (KeyValuePair<string, string> kvp in dic) {
			Obj.keys.Add (kvp.Key);
			Obj.list.Add (CreateStringObject (kvp.Value));
		}
		return Obj;
	}

	public static JSONObject Create (int[] vals)
	{
		JSONObject resultObj = Create ();
		resultObj.type = Type.ARRAY;
		resultObj.list = new List<JSONObject> ();
		foreach (int v in vals)
			resultObj.list.Add (Create (v));	
		return resultObj;
	}

	public static JSONObject CreateArrayJson (params int[] param)
	{
		return Create (param);
	}

	public JSONObject ()
	{
	}

	#region PARSE

	public JSONObject (string str, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
	{	//create a new JSONObject from a string (this will also create any children, and parse the whole string)
		Parse (str, maxDepth, storeExcessLevels, strict);
	}

	//	public static bool output_debug = false;

	void Parse (string inStr, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
	{
		if (!string.IsNullOrEmpty (inStr)) {
			inStr = inStr.Trim (WHITESPACE);
			if (strict) {
				if (inStr [0] != '[' && inStr [0] != '{') {
					type = Type.NULL;
					Debug.LogWarning ("Improper (strict) JSON formatting.  First character must be [ or {");
					return;
				}
			}
			if (inStr.Length > 0) {
				if (string.Compare (inStr, "true", true) == 0) {
					type = Type.BOOL;
					b = true;
				} else if (string.Compare (inStr, "false", true) == 0) {
					type = Type.BOOL;
					b = false;
				} else if (string.Compare (inStr, "null", true) == 0) {
					type = Type.NULL;
//#if USEFLOAT
				} else if (inStr == INFINITY) {
					type = Type.NUMBER;
					f = float.PositiveInfinity;
				} else if (inStr == NEGINFINITY) {
					type = Type.NUMBER;
					f = float.NegativeInfinity;
				} else if (inStr == NaN) {
					type = Type.NUMBER;
					f = float.NaN;
//#else
//            } else if(str == INFINITY) {
//                type = Type.NUMBER;
//                n = double.PositiveInfinity;
//            } else if(str == NEGINFINITY) {
//                type = Type.NUMBER;
//                n = double.NegativeInfinity;
//            } else if(str == NaN) {
//                type = Type.NUMBER;
//                n = double.NaN;
//#endif
				} else if (inStr [0] == '"') {
					type = Type.STRING;
					str = inStr.Substring (1, inStr.Length - 2);
					if (str.Length > 0 && str.Contains ("\\u")) {
						str = UnicodeUtil.Convert (str);
					}
				} else {
					int tokenTmp = 1;
					/*
					* Checking for the following formatting (www.json.org)
					* object - {"field1":value,"field2":value}
					* array - [value,value,value]
					* value - string	- "string"
					*		 - number	- 0.0
					*		 - bool		- true -or- false
					*		 - null		- null
					*/
					int offset = 0;
					switch (inStr [offset]) {
					case '{':
						type = Type.OBJECT;
						keys = new List<string> ();
						list = new List<JSONObject> ();
						break;
					case '[':
						type = Type.ARRAY;
						list = new List<JSONObject> ();
						break;
					default:
						try {
							#if USEFLOAT
							if (inStr.IndexOf ('.') >= 0 || inStr.IndexOf ('e') >= 0 || inStr.IndexOf ('E') >= 0) {
								f = System.Convert.ToSingle (inStr);
							} else {
								n = System.Convert.ToInt64 (inStr);
							}
							#else
								n = System.Convert.ToDouble(str);				 
							#endif
							type = Type.NUMBER;
						} catch (System.FormatException) {
							type = Type.NULL;
							//Debug.LogWarning ("improper JSON formatting:" + str);
						}
						return;
					}
					string propName = "";
					bool openQuote = false;
					bool inProp = false;
					int depth = 0;
					while (++offset < inStr.Length) {
						if (System.Array.IndexOf (WHITESPACE, inStr [offset]) > -1)
							continue;
						if (inStr [offset] == '\\')
							offset += 2;
						if (inStr [offset] == '"') {
							if (openQuote) {
								if (!inProp && depth == 0 && type == Type.OBJECT)
									propName = inStr.Substring (tokenTmp + 1, offset - tokenTmp - 1);
								openQuote = false;
							} else {
								if (depth == 0 && type == Type.OBJECT)
									tokenTmp = offset;
								openQuote = true;
							}
						}
						if (openQuote)
							continue;
						if (type == Type.OBJECT && depth == 0) {
							if (inStr [offset] == ':') {
								tokenTmp = offset + 1;
								inProp = true;
							}
						}

						if (inStr [offset] == '[' || inStr [offset] == '{') {
							depth++;
						} else if (inStr [offset] == ']' || inStr [offset] == '}') {
							depth--;
						}
						//if  (encounter a ',' at top level)  || a closing ]/}
						if ((inStr [offset] == ',' && depth == 0) || depth < 0) {
							inProp = false;
							string inner = inStr.Substring (tokenTmp, offset - tokenTmp).Trim (WHITESPACE);
							if (inner.Length > 0) {
//								if (output_debug) {
//									Log.D ("{0}", inner);
//									if (inner.CompareTo ("1442809048") == 0)
//										Log.D ("{0}", inner);
//								}
								if (type == Type.OBJECT)
									keys.Add (propName);
								if (maxDepth != -1)															//maxDepth of -1 is the end of the line
									list.Add (Create (inner, (maxDepth < -1) ? -2 : maxDepth - 1));
								else if (storeExcessLevels)
									list.Add (CreateBakedObject (inner));

							}
							tokenTmp = offset + 1;
						}
					}
				}
			} else
				type = Type.NULL;
		} else
			type = Type.NULL;	//If the string is missing, this is a null
		//Profiler.EndSample();
	}

	#endregion

	public bool IsNumber { get { return type == Type.NUMBER; } }

	public bool IsNull { get { return type == Type.NULL; } }

	public bool IsString { get { return type == Type.STRING; } }

	public bool IsBool { get { return type == Type.BOOL; } }

	public bool IsArray { get { return type == Type.ARRAY; } }

	public bool IsObject { get { return type == Type.OBJECT; } }

	public void Add (bool val)
	{
		Add (Create (val));
	}

	public void Add (float val)
	{
		Add (Create (val));
	}

    public void Add(long val)
    {
        Add(Create(val));
    }

    public void Add (int val)
	{
		Add (Create (val));
	}

	public void Add (string str)
	{
		Add (CreateStringObject (str));
	}

	public void Add (AddJSONConents content)
	{
		Add (Create (content));
	}

	public void Add (JSONObject obj)
	{
		if (obj) {		//Don't do anything if the object is null
			if (type != Type.ARRAY) {
				type = Type.ARRAY;		//Congratulations, son, you're an ARRAY now
				if (list == null)
					list = new List<JSONObject> ();
			}
			list.Add (obj);
		}
	}

	public void AddBoolField (string name, bool val)
	{
		AddField (name, Create (val));
	}

	public void AddField (string name, bool val)
	{
		AddField (name, Create (val));
	}

	public void AddField (string name, float val)
	{
		AddField (name, Create (val));
	}

	public void AddField (string name, int val)
	{
		AddField (name, Create (val));
	}

    public void AddField(string name, long val)
    {
        AddField(name, Create(val));
    }

    public void AddField (string name, AddJSONConents content)
	{
		AddField (name, Create (content));
	}

	public void AddField (string name, string val)
	{
		AddField (name, CreateStringObject (val));
	}

    public void AddField (string name,object val)
    {
        AddField(name, CreateObj(val));
    }

	public void AddField (string name, JSONObject obj)
	{
		if (obj) {		//Don't do anything if the object is null
			if (type != Type.OBJECT) {
				if (keys == null)
					keys = new List<string> ();
				if (type == Type.ARRAY) {
					for (int i = 0; i < list.Count; i++)
						keys.Add (i + "");
				} else if (list == null)
					list = new List<JSONObject> ();
				type = Type.OBJECT;		//Congratulations, son, you're an OBJECT now
			}
			keys.Add (name);
			list.Add (obj);
		}
	}

	public void SetField (string name, bool val)
	{
		SetField (name, Create (val));
	}

	public void SetField (string name, float val)
	{
		SetField (name, Create (val));
	}

	public void SetField (string name, int val)
	{
		SetField (name, Create (val));
	}

	public void SetField (string name, JSONObject obj)
	{
		if (HasField (name)) {
			list.Remove (this [name]);
			keys.Remove (name);
		}
		AddField (name, obj);
	}

	public void RemoveField (string name)
	{
		if (keys.IndexOf (name) > -1) {
			list.RemoveAt (keys.IndexOf (name));
			keys.Remove (name);
		}
	}

	public delegate void FieldNotFound (string name);

	public delegate void GetFieldResponse (JSONObject obj);

	public void GetField (ref bool field, string name, FieldNotFound fail = null)
	{
		if (type == Type.OBJECT) {
			int index = keys.IndexOf (name);
			if (index >= 0) {
				field = list [index].b;
				return;
			}
		}
		if (fail != null)
			fail.Invoke (name);
	}
	//#if USEFLOAT
	public void GetField (ref float field, string name, FieldNotFound fail = null)
	{
//#else
//public void GetField(ref double field, string name, FieldNotFound fail = null) {
//#endif
		if (type == Type.OBJECT) {
			int index = keys.IndexOf (name);
			if (index >= 0) {
				field = list [index].f;
				return;
			}
		}
		if (fail != null)
			fail.Invoke (name);
	}

	public void GetField (ref int field, string name, FieldNotFound fail = null)
	{
		if (type == Type.OBJECT) {
			int index = keys.IndexOf (name);
			if (index >= 0) {
				field = (int)list [index].n;
				return;
			}
		}
		if (fail != null)
			fail.Invoke (name);
	}

	public void GetField (ref uint field, string name, FieldNotFound fail = null)
	{
		if (type == Type.OBJECT) {
			int index = keys.IndexOf (name);
			if (index >= 0) {
				field = (uint)list [index].n;
				return;
			}
		}
		if (fail != null)
			fail.Invoke (name);
	}

	public void GetField (ref string field, string name, FieldNotFound fail = null)
	{
		if (type == Type.OBJECT) {
			int index = keys.IndexOf (name);
			if (index >= 0) {
				field = list [index].str;
				return;
			}
		}
		if (fail != null)
			fail.Invoke (name);
	}

	public void GetField (string name, GetFieldResponse response, FieldNotFound fail = null)
	{
		if (response != null && type == Type.OBJECT) {
			int index = keys.IndexOf (name);
			if (index >= 0) {
				response.Invoke (list [index]);
				return;
			}
		}
		if (fail != null)
			fail.Invoke (name);
	}

	public JSONObject GetField (string name)
	{
		if (type == Type.OBJECT)
			for (int i = 0; i < keys.Count; i++)
				if (keys [i] == name)
					return list [i];
		return null;
	}

	public bool HasFields (string[] names)
	{
		for (int i = 0; i < names.Length; i++)
			if (!keys.Contains (names [i]))
				return false;
		return true;
	}

	public bool HasField (string name)
	{
		if (type == Type.OBJECT)
			for (int i = 0; i < keys.Count; i++)
				if (keys [i] == name)
					return true;
		return false;
	}

	public void Clear ()
	{
		type = Type.NULL;
		if (list != null)
			list.Clear ();
		if (keys != null)
			keys.Clear ();
		str = "";
		n = 0;
		b = false;
	}

	/// <summary>
	/// Copy a JSONObject. This could probably work better
	/// </summary>
	/// <returns></returns>
	public JSONObject Copy ()
	{
		return Create (Print ());
	}
	/*
	* The Merge function is experimental. Use at your own risk.
	*/
	public void Merge (JSONObject obj)
	{
		MergeRecur (this, obj);
	}

	/// <summary>
	/// Merge object right into left recursively
	/// </summary>
	/// <param name="left">The left (base) object</param>
	/// <param name="right">The right (new) object</param>
	static void MergeRecur (JSONObject left, JSONObject right)
	{
		if (left.type == Type.NULL)
			left.Absorb (right);
		else if (left.type == Type.OBJECT && right.type == Type.OBJECT) {
			for (int i = 0; i < right.list.Count; i++) {
				string key = right.keys [i];
				if (right [i].isContainer) {
					if (left.HasField (key))
						MergeRecur (left [key], right [i]);
					else
						left.AddField (key, right [i]);
				} else {
					if (left.HasField (key))
						left.SetField (key, right [i]);
					else
						left.AddField (key, right [i]);
				}
			}
		} else if (left.type == Type.ARRAY && right.type == Type.ARRAY) {
			if (right.Count > left.Count) {
				Debug.LogError ("Cannot merge arrays when right object has more elements");
				return;
			}
			for (int i = 0; i < right.list.Count; i++) {
				if (left [i].type == right [i].type) {			//Only overwrite with the same type
					if (left [i].isContainer)
						MergeRecur (left [i], right [i]);
					else {
						left [i] = right [i];
					}
				}
			}
		}
	}

	public void Bake ()
	{
		if (type != Type.BAKED) {
			str = Print ();
			type = Type.BAKED;
		}
	}

	public IEnumerable BakeAsync ()
	{
		if (type != Type.BAKED) {
			foreach (string s in PrintAsync()) {
				if (s == null)
					yield return s;
				else {
					str = s;
				}
			}
			type = Type.BAKED;
		}
	}
	#pragma warning disable 219
	public string Print (bool pretty = false)
	{
		StringBuilder builder = new StringBuilder ();
		Stringify (0, builder, pretty);
		return builder.ToString ();
	}

	public IEnumerable<string> PrintAsync (bool pretty = false)
	{
		StringBuilder builder = new StringBuilder ();
		printWatch.Reset ();
		printWatch.Start ();
		foreach (IEnumerable e in StringifyAsync(0, builder, pretty)) {
			yield return null;
		}
		yield return builder.ToString ();
	}
	#pragma warning restore 219
	#region STRINGIFY

	const float maxFrameTime = 0.008f;
	static readonly Stopwatch printWatch = new Stopwatch ();

	IEnumerable StringifyAsync (int depth, StringBuilder builder, bool pretty = false)
	{	//Convert the JSONObject into a string
		//Profiler.BeginSample("JSONprint");
		if (depth++ > MAX_DEPTH) {
			Debug.Log ("reached max depth!");
			yield break;
		}
		if (printWatch.Elapsed.TotalSeconds > maxFrameTime) {
			printWatch.Reset ();
			yield return null;
			printWatch.Start ();
		}
		switch (type) {
		case Type.BAKED:
			builder.Append (str);
			break;
		case Type.STRING:
			builder.AppendFormat ("\"{0}\"", str);
			break;
		case Type.NUMBER:
//#if USEFLOAT
			if (float.IsInfinity (f))
				builder.Append (INFINITY);
			else if (float.IsNegativeInfinity (f))
				builder.Append (NEGINFINITY);
			else if (float.IsNaN (f))
				builder.Append (NaN);
//#else
//            if(double.IsInfinity(n))
//                builder.Append(INFINITY);
//            else if(double.IsNegativeInfinity(n))
//                builder.Append(NEGINFINITY);
//            else if(double.IsNaN(n))
//                builder.Append(NaN);
//#endif
			else {
				builder.Append (isIntNumber ? n.ToString () : f.ToString ());
			}
			break;
		case Type.OBJECT:
			builder.Append ("{");
			if (list.Count > 0) {
#if(PRETTY)	//for a bit more readability, comment the define above to disable system-wide
				if (pretty)
					builder.Append ("\n");
#endif
				for (int i = 0; i < list.Count; i++) {
					string key = keys [i];
					JSONObject obj = list [i];
					if (obj) {
#if(PRETTY)
						if (pretty)
							for (int j = 0; j < depth; j++)
								builder.Append ("\t"); //for a bit more readability
#endif
						builder.AppendFormat ("\"{0}\":", key);
						foreach (IEnumerable e in obj.StringifyAsync(depth, builder, pretty))
							yield return e;
						builder.Append (",");
#if(PRETTY)
						if (pretty)
							builder.Append ("\n");
#endif
					}
				}
//#if(PRETTY)
				if (pretty)
					builder.Length -= 2;
				else
//#endif
					builder.Length--;
			}
#if(PRETTY)
			if (pretty && list.Count > 0) {
				builder.Append ("\n");
				for (int j = 0; j < depth - 1; j++)
					builder.Append ("\t"); //for a bit more readability
			}
#endif
			builder.Append ("}");
			break;
		case Type.ARRAY:
			builder.Append ("[");
			if (list.Count > 0) {
#if(PRETTY)
				if (pretty)
					builder.Append ("\n"); //for a bit more readability
#endif
				for (int i = 0; i < list.Count; i++) {
					if (list [i]) {
#if(PRETTY)
						if (pretty)
							for (int j = 0; j < depth; j++)
								builder.Append ("\t"); //for a bit more readability
#endif
						foreach (IEnumerable e in list[i].StringifyAsync(depth, builder, pretty))
							yield return e;
						builder.Append (",");
#if(PRETTY)
						if (pretty)
							builder.Append ("\n"); //for a bit more readability
#endif
					}
				}
//#if(PRETTY)
				if (pretty)
					builder.Length -= 2;
				else
//#endif
					builder.Length--;
			}
#if(PRETTY)
			if (pretty && list.Count > 0) {
				builder.Append ("\n");
				for (int j = 0; j < depth - 1; j++)
					builder.Append ("\t"); //for a bit more readability
			}
#endif
			builder.Append ("]");
			break;
		case Type.BOOL:
			if (b)
				builder.Append ("true");
			else
				builder.Append ("false");
			break;
		case Type.NULL:
			builder.Append ("null");
			break;
		}
		//Profiler.EndSample();
	}
	//TODO: Refactor Stringify functions to share core logic
	/*
	* I know, I know, this is really bad form.  It turns out that there is a
	* significant amount of garbage created when calling as a coroutine, so this
	* method is duplicated.  Hopefully there won't be too many future changes, but
	* I would still like a more elegant way to optionaly yield
	*/
	void Stringify (int depth, StringBuilder builder, bool pretty = false)
	{	//Convert the JSONObject into a string
		//Profiler.BeginSample("JSONprint");
		if (depth++ > MAX_DEPTH) {
			Debug.Log ("reached max depth!");
			return;
		}
		switch (type) {
		case Type.BAKED:
			builder.Append (str);
			break;
		case Type.STRING:
			builder.AppendFormat ("\"{0}\"", str);
			break;
		case Type.NUMBER:
		case Type.LONG:
//#if USEFLOAT
			if (float.IsInfinity (f))
				builder.Append (INFINITY);
			else if (float.IsNegativeInfinity (f))
				builder.Append (NEGINFINITY);
			else if (float.IsNaN (f))
				builder.Append (NaN);
//#else
//            if(double.IsInfinity(n))
//                builder.Append(INFINITY);
//            else if(double.IsNegativeInfinity(n))
//                builder.Append(NEGINFINITY);
//            else if(double.IsNaN(n))
//                builder.Append(NaN);
//#endif
			else
				builder.Append (isIntNumber ? n.ToString () : f.ToString ());
			break;
		case Type.OBJECT:
			builder.Append ("{");
			if (list.Count > 0) {
#if(PRETTY)	//for a bit more readability, comment the define above to disable system-wide
				if (pretty)
					builder.Append ("\n");
#endif
				for (int i = 0; i < list.Count; i++) {
					string key = keys [i];
					JSONObject obj = list [i];
					if (obj) {
#if(PRETTY)
						if (pretty)
							for (int j = 0; j < depth; j++)
								builder.Append ("\t"); //for a bit more readability
#endif
						builder.AppendFormat ("\"{0}\":", key);
						obj.Stringify (depth, builder, pretty);
						builder.Append (",");
#if(PRETTY)
						if (pretty)
							builder.Append ("\n");
#endif
					}
				}
//#if(PRETTY)
				if (pretty)
					builder.Length -= 2;
				else
//#endif
					builder.Length--;
			}
#if(PRETTY)
			if (pretty && list.Count > 0) {
				builder.Append ("\n");
				for (int j = 0; j < depth - 1; j++)
					builder.Append ("\t"); //for a bit more readability
			}
#endif
			builder.Append ("}");
			break;
		case Type.ARRAY:
			builder.Append ("[");
			if (list.Count > 0) {
#if(PRETTY)
				if (pretty)
					builder.Append ("\n"); //for a bit more readability
#endif
				for (int i = 0; i < list.Count; i++) {
					if (list [i]) {
#if(PRETTY)
						if (pretty)
							for (int j = 0; j < depth; j++)
								builder.Append ("\t"); //for a bit more readability
#endif
						list [i].Stringify (depth, builder, pretty);
						builder.Append (",");
#if(PRETTY)
						if (pretty)
							builder.Append ("\n"); //for a bit more readability
#endif
					}
				}
//#if(PRETTY)
				if (pretty)
					builder.Length -= 2;
				else
//#endif
					builder.Length--;
			}
#if(PRETTY)
			if (pretty && list.Count > 0) {
				builder.Append ("\n");
				for (int j = 0; j < depth - 1; j++)
					builder.Append ("\t"); //for a bit more readability
			}
#endif
			builder.Append ("]");
			break;
		case Type.BOOL:
			if (b)
				builder.Append ("true");
			else
				builder.Append ("false");
			break;
		case Type.NULL:
			builder.Append ("null");
			break;
		}
		//Profiler.EndSample();
	}

	#endregion

	public static implicit operator WWWForm (JSONObject obj)
	{
		WWWForm form = new WWWForm ();
		for (int i = 0; i < obj.list.Count; i++) {
			string key = i + "";
			if (obj.type == Type.OBJECT)
				key = obj.keys [i];
			string val = obj.list [i].ToString ();
			if (obj.list [i].type == Type.STRING)
				val = val.Replace ("\"", "");
			form.AddField (key, val);
		}
		return form;
	}

	public JSONObject this [int index] {
		get {
			if (list.Count > index)
				return list [index];
			return null;
		}
		set {
			if (list.Count > index)
				list [index] = value;
		}
	}

	public JSONObject this [string index] {
		get {
			return GetField (index);
		}
		set {
			SetField (index, value);
		}
	}

	public override string ToString ()
	{
		return Print ();
	}

	public string ToString (bool pretty)
	{
		return Print (pretty);
	}

	public Dictionary<string, string> ToDictionary ()
	{
		if (type == Type.OBJECT) {
			Dictionary<string, string> result = new Dictionary<string, string> ();
			for (int i = 0; i < list.Count; i++) {
				JSONObject val = list [i];
				switch (val.type) {
				case Type.STRING:
					result.Add (keys [i], val.str);
					break;
				case Type.NUMBER:
					result.Add (keys [i], val.f + "");
					break;
				case Type.BOOL:
					result.Add (keys [i], val.b + "");
					break;
				default:
					Debug.LogWarning ("Omitting object: " + keys [i] + " in dictionary conversion");
					break;
				}
			}
			return result;
		}
		Debug.LogWarning ("Tried to turn non-Object JSONObject into a dictionary");
		return null;
	}

	public static implicit operator bool (JSONObject o)
	{
		return o != null;
	}

	public static JSONObject toJSONObject (object o)
	{
		IList asList;
		IDictionary asDict;
		string asStr;
		//Log.D ("Test,o:{0}", o);
		if (o == null) {
			return new JSONObject (Type.NULL);
		} else if ((asStr = o as string) != null) {
			return CreateStringObject (asStr);
		} else if (o is bool) {
			return new JSONObject ((bool)o);
		} else if ((asList = o as IList) != null) {
			//Log.D("a4,o:{0},asList{1}",o,asList);
			JSONObject jList = new JSONObject (Type.ARRAY);
			for (int i = 0; i < asList.Count; i++) { 
				jList.Add (toJSONObject (asList [i]));
			}
			//Log.D("jList:{0}",jList);
			return jList;
		} else if ((asDict = o as IDictionary) != null) {
			JSONObject jDict = new JSONObject (Type.OBJECT);

			foreach (string key in asDict.Keys) { 
				jDict.AddField (key, toJSONObject (asDict [key]));
			}

			return jDict;
		} else if (o is char) {
			return CreateStringObject (o.ToString ());
		} else if (o is float) {
			//Log.D("[ float ]v:{0}, type:{1}.", o, o.GetType());
			return new JSONObject ((float)o);
		} else if (o is int) {
			//Log.D("[ int ]v:{0}, type:{1}.", o, o.GetType());
			return new JSONObject ((int)o);
		} else if (o is long) {
			//Log.D("[ long ]v:{0}, float:{2}, type:{1}.", o, o.GetType(), (long)o);
			return new JSONObject ((long)o);
		}else if(o is double)
        {
            return new JSONObject((float)(double)o);
        }
        else {
            throw new System.Exception();
			//throw new MessagePackException (string.Format ("[ toJSONObject ]unsupported type:{0}.", o.GetType ()));
		}
	}

	#if POOLING
	static bool pool = true;
public static void ClearPool() {
	pool = false;
	releaseQueue.Clear();
	pool = true;
}

~JSONObject() {
	if(pool && releaseQueue.Count < MAX_POOL_SIZE) {
		type = Type.NULL;
		list = null;
		keys = null;
		str = "";
		n = 0;
		b = false;
		releaseQueue.Enqueue(this);
	}
}
#endif

	public float ParseF ()
	{
		return IsString ? float.Parse (str) : 0f;	
	}

	public static Vector3 ParseVector3FromJson (JSONObject json)
	{
		try {
			return new Vector3 (float.Parse (json [0].str), float.Parse (json [1].str), float.Parse (json [2].str));
		} catch {
			return Vector3.zero;
		}
	}

	public static Vector3 ParseVector2FromJson (JSONObject json)
	{
		try {
			return new Vector2 (float.Parse (json [0].str), float.Parse (json [1].str));
		} catch {
			return Vector2.zero;
		}
	}

	public static float[] ParseFloatArrayFromJson (JSONObject json)
	{
		try {
			float[] result = new float[json.Count];

			for (int i = 0; i < result.Length; i++) {
				result [i] = float.Parse (json [i].str);
			}

			return result;
		} catch {
			return new float[0];
		}
	}

}
