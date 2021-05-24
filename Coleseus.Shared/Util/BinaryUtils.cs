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

public class BinaryUtils
{

	private static const String HEXES = "0123456789ABCDEF";

	public static String getHexString(byte[] raw)
	{
		return getHexString(raw, null);
	}

	public static String getHexString(byte[] raw, String separator)
	{
		bool sep = (null != separator) && !("".equals(separator));

		if (raw == null)
		{
			return null;
		}
		var StringBuilder hex = new StringBuilder(2 * raw.length);
		for (var byte b : raw)
		{
			hex.append(HEXES.charAt((b & 0xF0) >> 4)).append(
					HEXES.charAt((b & 0x0F)));
			if (sep)
			{
				hex.append(separator);
			}
		}
		return hex.toString();
	}
}
