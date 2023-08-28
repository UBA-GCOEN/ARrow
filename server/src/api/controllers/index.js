import jwt from 'jsonwebtoken'
 
 const indexController = async (req, res) => {

     /**
      * check if there is already any session 
      * if yes get the session data and login
      * else redirect to landing page
      */
    if(req.session.user){

        let isTokenValid = jwt.decode(req.session.user.token)
        if(isTokenValid){
            res.status(200).json({
                success: true,
                user: req.session.user,
                msg: req.session.user.user.name +" is logged in successfully"
              });
        }

    }
    else{
         //show landing page
         res.status(200).json({message:"Arrow Server is running..."})
    }
    


}

export default indexController;