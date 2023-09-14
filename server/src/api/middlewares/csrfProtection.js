import csrf from 'csurf'

const csrfProtection = csrf({ cookie: false });

// Custom CSRF middleware function
export  const csrfProtect = (req, res, next) => {

    if(req.session.csrfToken){


        /**
         * for development only 
         * csrf token is provided 
         * in all login response 
         * It is harmful in production.
         * 
         * remove below line in production.  
         */
        req.csrfToken = req.session.csrfToken


        csrfProtection(req, res, (err) => {
            if (err) {
              return res.status(403).json({ err });
            }
            
          });
          next();
    }
    else{

       
        csrfProtection(req, res, (err) => {
            if (err) {
              
            }
          });
        const csrfToken = req.csrfToken();
        req.session.csrfToken = csrfToken
        // Attach the CSRF token to the response headers or in the response body, as needed
        req.csrfToken = csrfToken
      
        next();
    }

};




