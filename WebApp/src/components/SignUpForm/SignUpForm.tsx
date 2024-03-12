import React, { useState } from 'react';

function SignUpForm() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const handleSignUp = () => {
    
  };

  const handleSignIn = () => {
    
  };

  return (
    <div style={{ width: '30%', margin: '0 auto' ,marginTop:'7%',marginBottom:'7%',color:'rgb(60, 60, 60)', fontFamily: 'Cooper Black, sans-serif' }}> 
      <div style={{ display: 'flex', justifyContent: 'space-between', borderRadius: '20px', overflow: 'hidden', marginBottom: '20px' }}>
        <button style={{ width: '50%', backgroundColor: 'grey', border: '1px solid grey', padding: '10px', borderRadius: '20px 0 0 20px', borderColor: 'grey',color:'rgb(60, 60, 60)', fontFamily: 'Cooper Black, sans-serif' }} onClick={handleSignUp}>SIGN UP</button>
        <button style={{ width: '50%', backgroundColor: 'white', border: '1px solid grey', padding: '10px', borderRadius: '0 20px 20px 0', borderColor: 'grey' ,color:'rgb(60, 60, 60)', fontFamily: 'Cooper Black, sans-serif'}} onClick={handleSignIn}>SIGN IN</button>
      </div>
      <div style={{ borderRadius: '20px', backgroundColor: '#f2f2f2', padding: '20px', marginBottom: '20px', border: '1px solid grey', borderColor: 'grey' }}>
        <div style={{ marginBottom: '10px' }}>
          <label>Email</label>
          <br />
          <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} style={{border: '1px solid grey', borderRadius: '4px', padding: '5px'}}/>
        </div>
        <div style={{ marginBottom: '10px' }}>
          <label>Password</label>
          <br />
          <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} style={{border: '1px solid grey', borderRadius: '4px', padding: '5px'}}/>
        </div>
        <div style={{ marginBottom: '10px' }}>
          <label>Confirm Password</label>
          <br />
          <input type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} style={{border: '1px solid grey', borderRadius: '4px', padding: '5px'}}/>
        </div>
      </div>
      <div style={{ borderRadius: '20px', overflow: 'hidden' }}>
        <button style={{ width: '100%', backgroundColor: 'white', padding: '10px', borderRadius: '20px', border: '1px solid grey', borderColor: 'grey',color:'rgb(60, 60, 60)', fontFamily: 'Cooper Black, sans-serif' }}>Submit</button>
      </div>
    </div>
  );
}

export default SignUpForm;
