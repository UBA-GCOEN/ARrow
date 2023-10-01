import userModel from "../models/userModel.js"


/**
 * Route: /profile
 * Desc: get profile details 
 */
export const profile = async (req, res) => {

    res.send("Display profile page")
}


/**
 * Route: /profile/update
 * Desc: request to update profile
 */
export const updateProfile = async (req, res) => {

    const { name, email} = req.body

    const emailDomains = [
        "@gmail.com",
        "@yahoo.com",
        "@hotmail.com",
        "@aol.com",
        "@outlook.com",
        ];

    var result = {}    
    var newResult={}
 
 
 
        //check name length
        if (name.length < 2) {
          return res
            .status(404)
            .json({ message: "Name must be atleast 2 characters long." });
        }
 
        // check email format
        if (!emailDomains.some((v) => email.indexOf(v) >= 0)) {
          return res.status(404).json({
         message: "Please enter a valid email address",
       })};




     /**
     * getting remaing fields 
     * from requests.
     */
     var {
        branch,
        subjects, 
        intrest,
        enrollNo,
        designation,
        bio,
        education,
        intrest,
        mobile,
     } = req.body



       /**
        * checking field types
        * to avoid sql attacks
        */
       if (typeof name !== "string") {
        res.status(400).json({ status: "error"+name+ typeof name });
        return;
      }

      if (typeof email !== "string") {
        res.status(400).json({ status: "error"+email+typeof email });
        return;
      }
       
      if (typeof branch !== "string") {
        res.status(400).json({ status: "error"+branch+typeof branch });
        return;
      }

      if (typeof subjects !== "string") {
        res.status(400).json({ status: "error"+subjects+typeof subjects });
        return;
      }

      if (typeof enrollNo !== "number") {
        res.status(400).json({ status: "error"+enrollNo+typeof enrollNo });
        return;
      }

      if (typeof designation !== "string") {
        res.status(400).json({ status: "error"+designation+typeof designation });
        return;
      }

      if (typeof bio !== "string") {
        res.status(400).json({ status: "error"+bio+typeof bio });
        return;
      }

      if (typeof education !== "string") {
        res.status(400).json({ status: "error"+education+typeof education });
        return;
      }

      if (typeof intrest !== "string") {
        res.status(400).json({ status: "error"+intrest+typeof intrest});
        return;
      }

      if (typeof mobile !== "number") {
        res.status(400).json({ status: "error"+mobile+typeof mobile });
        return;
      }
      
      // if (typeof role !== "string") {
      //   res.status(400).json({ status: "error"+role+typeof role });
      //   return;
      // }




        //update User in database
        result = await userModel.updateOne({
            name,
            email,
            branch,
            subjects,
            designation,
            enrollNo,
            education,
            bio,
            intrest,
            mobile,
         });

         
         newResult = await userModel.findOne({email})



    if(result){

        
        res.json({
            success: true,
            msg: "user "+newResult.role+" updated successfully",
            user: newResult
    })
     }
    
}
