// Copyright 2021 pearl2201
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

<<<<<<< HEAD
public class ColyseusConfig
{
    public const string NODE_NAME = "NadNode";
    public const string RECONNECT_KEY = "RECONNECT_KEY";
    public const string RECONNECT_REGISTRY = "RECONNECT_REGISTRY";
    /**
	 * By default wait for 5 minutes for remote client to reconnect, before
	 * closing session.
	 */
    public const int DEFAULT_RECONNECT_DELAY = 5 * 60 * 1000;
=======
public class ColyseusConfig 
{
	public static const String NODE_NAME = "NadNode";
	public static const String RECONNECT_KEY = "RECONNECT_KEY";
	public static const String RECONNECT_REGISTRY = "RECONNECT_REGISTRY";
	/**
	 * By default wait for 5 minutes for remote client to reconnect, before
	 * closing session.
	 */
	public static const int DEFAULT_RECONNECT_DELAY =  5 * 60 * 1000;
>>>>>>> 40b1c90824edfea1c764b751e0a46fdb0a7d1df1
}