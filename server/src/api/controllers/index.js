import jwt from 'jsonwebtoken'
 
 const indexController = async (req, res) => {

     /**
      * check if there is already any session 
      * if yes get the session data and login
      * else redirect to landing page
      */


    // if(req.session.user){
    //     let isTokenValid = jwt.decode(req.session.user.token)
    //     if(isTokenValid){
    //         res.status(200).json({
    //             success: true,
    //             user: req.session.user,
    //             msg: req.session.user.user.name +" is logged in successfully"
    //           });
    //     }
    // }
    // else{
    //      //show landing page
    //      res.status(200).json({message:"Arrow Server is running..."})
    // }
    

    /**
     *  If we find the way in frontend to route the user 
     *  to homepage if there is any one already logged in else route to home page
     *  then we won't need above part and can be deleted
     */

    // Incase we find a way remove below response as well
    res.status(200).json({message:"Arrow Server is running..."})


}

export default indexController;