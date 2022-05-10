import React, { useState } from 'react';
import PropTypes from 'prop-types';
import './Login.scss';

async function loginUser(username: string | null, password: string | null) {
 return fetch('http://localhost:8080/login', {
   method: 'POST',
   headers: {
     'Content-Type': 'application/json'
   },
   body: JSON.stringify({username: username, password: password})
 })
   .then(data => data.json())
}

export default function Login(props: any) {
  const [username, setUserName] = useState<string | null>("");
  const [password, setPassword] = useState<string | null>("");

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const token = await loginUser(username, password);
    props.setToken(token);
  }

  return(
    <div className="login-wrapper">
      <h1>Please Log In</h1>
      <form onSubmit={handleSubmit}>
        <label>
          <p>Username</p>
          <input type="text" onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserName(e.target.value)} />
        </label>
        <label>
          <p>Password</p>
          <input type="password" onChange={(e: React.ChangeEvent<HTMLInputElement>) => setPassword(e.target.value)} />
        </label>
        <div>
          <button type="submit">Submit</button>
        </div>
      </form>
    </div>
  )
}

Login.propTypes = {
  setToken: PropTypes.func.isRequired
};