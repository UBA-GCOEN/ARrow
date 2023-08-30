/**
 * Route: /logout
 * Desc: logout
 */

export const logout = (req, res)=> {
    if(req.session.user){
        req.session.destroy(err => {
            if (err) {
              console.error("Error destroying session:", err);
              res.status(500).send("Internal Server Error");
            } else {
              res.send("Logged out successfully");
            }
          });
    }
    else{
        res.send("No Active sessions")
    }

  }; 