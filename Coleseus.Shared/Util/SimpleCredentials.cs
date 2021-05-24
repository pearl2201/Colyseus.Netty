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
public class SimpleCredentials : Credentials
{
    private  string username;
	private string password;
	
	public SimpleCredentials(string username, string password)
    {
        this.username = username;
        this.password = password;
    }

    public SimpleCredentials(ByteBuf buffer)
    {
        this.username = NettyUtils.readString(buffer);
        this.password = NettyUtils.readString(buffer);
    }

  
    public string getUsername()
    {
        return username;
    }

   
    public string getPassword()
    {
        return password;
    }


    public override string ToString()
    {
        return username;
    }
=======
public class SimpleCredentials implements Credentials
{
	private final String username;
	private final String password;
	
	public SimpleCredentials(String username, String password)
	{
		this.username = username;
		this.password = password;
	}
	
	public SimpleCredentials(ByteBuf buffer)
	{
		this.username = NettyUtils.readString(buffer);
		this.password = NettyUtils.readString(buffer);
	}

	@Override
	public String getUsername()
	{
		return username;
	}

	@Override
	public String getPassword()
	{
		return password;
	}

	@Override
	public String toString()
	{
		return username;
	}
>>>>>>> 40b1c90824edfea1c764b751e0a46fdb0a7d1df1
}