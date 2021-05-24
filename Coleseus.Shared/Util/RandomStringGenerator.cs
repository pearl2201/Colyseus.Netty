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
using System;
using Serilog;

public class RandomStringGenerator
{
    public const int DEFAULT_LENGTH = 8;

    static char[] alphaNumberic = new char[] { 'A', 'B', 'C', 'D', 'E', 'F',
            'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S',
            'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
            't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6',
            '7', '8', '9', '0' };

    public static string generateRandomString(int length)
    {
        string random = "ACK";

        int len = DEFAULT_LENGTH;
        if (length > 0)
        {
            len = length;
        }

        char[] randomChars = new char[len];
        try
        {
            Random r = new Random(); ;
            for (int i = 0; i < len; i++)
            {
                int nextChar = r.Next(alphaNumberic.Length);
                randomChars[i] = alphaNumberic[nextChar];
            }
            random = new string(randomChars);
        }
        catch (Exception e)
        {
            // TODO Auto-generated catch block
            Log.Error("{Exception}", e);
        }

        return random;
    }
=======
public class RandomStringGenerator
{
	public static final int DEFAULT_LENGTH = 8;

	static char[] alphaNumberic = new char[] { 'A', 'B', 'C', 'D', 'E', 'F',
			'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S',
			'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
			'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
			't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6',
			'7', '8', '9', '0' };

	public static String generateRandomString(int length)
	{
		String random = "ACK";
		
		int len = DEFAULT_LENGTH;
		if (length > 0)
		{
			len = length;
		}
		
		char[] randomChars = new char[len];
		try
		{
			SecureRandom wheel = SecureRandom.getInstance("SHA1PRNG");
			for (int i = 0; i < len; i++)
			{
				int nextChar = wheel.nextInt(alphaNumberic.length);
				randomChars[i] = alphaNumberic[nextChar];
			}
			random = new String(randomChars);
		}
		catch (NoSuchAlgorithmException e)
		{
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		return random;
	}
>>>>>>> 40b1c90824edfea1c764b751e0a46fdb0a7d1df1
}