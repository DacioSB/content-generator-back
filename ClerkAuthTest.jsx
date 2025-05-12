import React, { useState, useEffect } from 'react';
import { useAuth, useUser } from '@clerk/clerk-react';

const API_URL = 'https://localhost:7001'; // Update with your API URL

const ClerkAuthTest = () => {
  const { getToken, isSignedIn } = useAuth();
  const { user } = useUser();
  const [apiResponse, setApiResponse] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchPublicEndpoint = async () => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await fetch(`${API_URL}/api/auth/public`);
      const data = await response.json();
      setApiResponse(data);
    } catch (err) {
      setError(`Error fetching public endpoint: ${err.message}`);
      console.error('Error fetching public endpoint:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchProtectedEndpoint = async () => {
    if (!isSignedIn) {
      setError('You must be signed in to access protected endpoints');
      return;
    }

    setLoading(true);
    setError(null);
    
    try {
      // Get the JWT token from Clerk
      const token = await getToken();
      
      const response = await fetch(`${API_URL}/api/auth/protected`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (!response.ok) {
        throw new Error(`API returned ${response.status}: ${response.statusText}`);
      }
      
      const data = await response.json();
      setApiResponse(data);
    } catch (err) {
      setError(`Error fetching protected endpoint: ${err.message}`);
      console.error('Error fetching protected endpoint:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchUserInfo = async () => {
    if (!isSignedIn) {
      setError('You must be signed in to access user info');
      return;
    }

    setLoading(true);
    setError(null);
    
    try {
      // Get the JWT token from Clerk
      const token = await getToken();
      
      const response = await fetch(`${API_URL}/api/auth/user-info`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (!response.ok) {
        throw new Error(`API returned ${response.status}: ${response.statusText}`);
      }
      
      const data = await response.json();
      setApiResponse(data);
    } catch (err) {
      setError(`Error fetching user info: ${err.message}`);
      console.error('Error fetching user info:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchUserProjects = async () => {
    if (!isSignedIn) {
      setError('You must be signed in to access your projects');
      return;
    }

    setLoading(true);
    setError(null);
    
    try {
      // Get the JWT token from Clerk
      const token = await getToken();
      
      const response = await fetch(`${API_URL}/api/projects`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (!response.ok) {
        throw new Error(`API returned ${response.status}: ${response.statusText}`);
      }
      
      const data = await response.json();
      setApiResponse(data);
    } catch (err) {
      setError(`Error fetching projects: ${err.message}`);
      console.error('Error fetching projects:', err);
    } finally {
      setLoading(false);
    }
  };

  const createProject = async () => {
    if (!isSignedIn) {
      setError('You must be signed in to create a project');
      return;
    }

    setLoading(true);
    setError(null);
    
    try {
      // Get the JWT token from Clerk
      const token = await getToken();
      
      const response = await fetch(`${API_URL}/api/projects`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          name: `Test Project ${new Date().toISOString()}`,
          isPublic: true
        })
      });
      
      if (!response.ok) {
        throw new Error(`API returned ${response.status}: ${response.statusText}`);
      }
      
      const data = await response.json();
      setApiResponse(data);
    } catch (err) {
      setError(`Error creating project: ${err.message}`);
      console.error('Error creating project:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="clerk-auth-test">
      <h1>Clerk Authentication Test</h1>
      
      <div className="user-info">
        <h2>User Information</h2>
        {isSignedIn ? (
          <div>
            <p><strong>Signed in as:</strong> {user.primaryEmailAddress?.emailAddress}</p>
            <p><strong>User ID:</strong> {user.id}</p>
          </div>
        ) : (
          <p>Not signed in. Please sign in to test protected endpoints.</p>
        )}
      </div>
      
      <div className="api-tests">
        <h2>API Tests</h2>
        <div className="buttons">
          <button onClick={fetchPublicEndpoint} disabled={loading}>
            Test Public Endpoint
          </button>
          <button onClick={fetchProtectedEndpoint} disabled={loading || !isSignedIn}>
            Test Protected Endpoint
          </button>
          <button onClick={fetchUserInfo} disabled={loading || !isSignedIn}>
            Get User Info
          </button>
          <button onClick={fetchUserProjects} disabled={loading || !isSignedIn}>
            Get My Projects
          </button>
          <button onClick={createProject} disabled={loading || !isSignedIn}>
            Create Test Project
          </button>
        </div>
        
        {loading && <p>Loading...</p>}
        {error && <p className="error">{error}</p>}
        
        {apiResponse && (
          <div className="api-response">
            <h3>API Response:</h3>
            <pre>{JSON.stringify(apiResponse, null, 2)}</pre>
          </div>
        )}
      </div>
      
      <style jsx>{`
        .clerk-auth-test {
          max-width: 800px;
          margin: 0 auto;
          padding: 20px;
          font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        }
        
        .buttons {
          display: flex;
          flex-wrap: wrap;
          gap: 10px;
          margin-bottom: 20px;
        }
        
        button {
          padding: 8px 16px;
          background-color: #0070f3;
          color: white;
          border: none;
          border-radius: 4px;
          cursor: pointer;
        }
        
        button:disabled {
          background-color: #ccc;
          cursor: not-allowed;
        }
        
        .error {
          color: red;
          font-weight: bold;
        }
        
        .api-response {
          background-color: #f5f5f5;
          padding: 15px;
          border-radius: 4px;
          overflow: auto;
        }
        
        pre {
          margin: 0;
        }
      `}</style>
    </div>
  );
};

export default ClerkAuthTest;
